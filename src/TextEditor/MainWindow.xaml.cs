using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Controls;
using TextEditor.Enums;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Globalization;
using TextEditor.Controls;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Document _document;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sideScrollDrawer = new ScrollBarDrawer();
            var scrollBar = new SideScroll(sideScrollDrawer);
            SideScrollContainer.Children.Add(scrollBar);
            scrollBar.Scrolled += ScrollBar_Scrolled;
            
            _document = new Document(Main, sideScrollDrawer);
            fontSizeInfo.Text = $"Font size: {_document.FontSize}px";

            RefreshTextInfo();
        }

        private void ScrollBar_Scrolled(int scrolledToPercentage)
        {
            _document.MoveDisplay(scrolledToPercentage);
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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _document.SaveFile(dialog.FileName);
            title.Text = dialog.FileName;

            textInfo.Text = string.Empty;
            textInfo.Inlines.Add("Document saved to ");
            textInfo.Inlines.Add(new Run(dialog.FileName) { FontWeight = FontWeights.Bold });
            await Task.Delay(4000);
            RefreshTextInfo();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "All files|*.*|Text files|*.txt|Json files| *.json";

            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
                return;

            _document.LoadFile(dialog.FileName);
            title.Text = dialog.FileName;
            RefreshTextInfo();
        }

        private void Main_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = (Canvas)sender;
            Point mousePosition = e.GetPosition(parent);
            var yPercentage = Math.Floor(mousePosition.Y / parent.ActualHeight * 100);
            //var xPercentage = (int)Math.Floor(mousePosition.X / parent.ActualWidth * 100);
            _document.MoveCursor(mousePosition.X, yPercentage);
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
