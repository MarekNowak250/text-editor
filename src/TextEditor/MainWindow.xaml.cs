using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Controls;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Document _document;
        private bool _scrolling = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _document = new Document(Main, new ScrollBarDrawer(SideScroll));
            fontSizeInfo.Text = $"Font size: {_document.FontSize}px";
            RefreshTextInfo();
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
            {
                if (c == '\b' || c == '\r')
                    return;

                _document.InsertChar(c);
            }
            RefreshTextInfo();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.Z)
                    _document.Undo();
                if(e.Key == Key.Y)
                    _document.Redo();

                RefreshTextInfo();
                return;
            }

            switch (e.Key)
            {
                case Key.Back:
                case Key.Delete:
                    _document.DeleteChar();
                    break;
                case Key.Enter:
                    _document.AddLine();
                    break;
                case Key.Left:
                    _document.MoveCursor(Direction.Left, false);
                    break;
                case Key.Up:
                    _document.MoveCursor(Direction.Up, false);
                    break;
                case Key.Right:
                    _document.MoveCursor(Direction.Right, false);
                    break;
                case Key.Down:
                    _document.MoveCursor(Direction.Down, false);
                    break;
                default:
                    return;
            }
            RefreshTextInfo();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _document.SaveFile(dialog.FileName);
            title.Text = dialog.FileName;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = "txt";

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _document.LoadFile(dialog.FileName);
            title.Text = dialog.FileName;
            RefreshTextInfo();
        }

        private void SideScroll_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _scrolling = true;
            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            _document.MoveDisplay((int)Math.Floor(mousePosition.Y / parent.ActualHeight * 100));
        }

        private void Main_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            var yPercentage = Math.Floor(mousePosition.Y / parent.ActualHeight * 100);
            //var xPercentage = (int)Math.Floor(mousePosition.X / parent.ActualWidth * 100);
            _document.MoveCursor(mousePosition.X, yPercentage);
        }

        private void SideScroll_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _scrolling = false;
        }

        private void SideScroll_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_scrolling)
                return;

            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            _document.MoveDisplay((int)Math.Floor(mousePosition.Y / parent.ActualHeight * 100));
        }

        private void SideScroll_MouseLeave(object sender, MouseEventArgs e)
        {
            _scrolling = false;
        }

        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var fontSize = _document.Zoom(e.Delta);
                fontSizeInfo.Text = $"Font size: {fontSize}px";
            }
            else
                _document.MoveDisplayDelta(e.Delta);
        }

        private void RefreshTextInfo()
        {
            textInfo.Text = $"{_document.RowCount} rows | {_document.CharCount} chars";
        }
    }
}
