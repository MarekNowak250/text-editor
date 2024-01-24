using System.Windows;
using System.Windows.Input;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Drawer _drawer;
        private Document _document;


        public MainWindow()
        {
            InitializeComponent();

            _drawer = new(new CharFactory(new System.Drawing.Font("Helvetica", 12F)), Main);
            _document = new Document();
            _drawer.Draw(_document.GetChars);
        }

        private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
            {
                if (c == '\b' || c== '\r')
                    return;

                _document.InsertChar(c);
                _drawer.Draw(_document.GetChars);
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

            _drawer.Draw(_document.GetChars);
        }
    }
}
