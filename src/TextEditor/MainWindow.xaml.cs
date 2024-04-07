using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Input;

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
            _document = new Document(Main);
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
            {
                if (c == '\b' || c== '\r')
                    return;

                _document.InsertChar(c);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
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
                    _document.MoveCursor(Direction.Left);
                    break;
                case Key.Up:
                    _document.MoveCursor(Direction.Up);
                    break;
                case Key.Right:
                    _document.MoveCursor(Direction.Right);
                    break;
                case Key.Down:
                    _document.MoveCursor(Direction.Down);
                    break;
                default:
                    return;
            }
        }
    }
}
