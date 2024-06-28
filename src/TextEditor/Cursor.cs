using System.Drawing;
using System.Windows.Media.Imaging;

namespace TextEditor
{
    public class Cursor : DocumentChar
    {
        public void SetColumn(int newCol)
        {
            Column = newCol;
        }

        public void SetRow(int newRow)
        {
            Row = newRow;
        }

        private CharFactory _charFactory;

        public Cursor(char character, int row, int column, float fontSize) : base(character, row, column)
        {
            _charFactory = new CharFactory(new Font("Courier New", fontSize, FontStyle.Bold));
        }

        public void ChangeFontSize(float newFontSize)
        {
            _charFactory = new CharFactory(new Font("Courier New", newFontSize, FontStyle.Bold));
        }

        public BitmapImage GetRender(Font font = null)
        {
            return _charFactory.GetCharRender(Character, font);
        }
    }
}
