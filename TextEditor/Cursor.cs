using System.Collections.Generic;

namespace TextEditor
{
    internal class Cursor : DocumentChar
    {
        public int DisplayColumn { get; set; }
        public int DisplayRow { get; set; }

        public void SetColumn(int newCol)
        {
            Column = newCol;
        }

        public void SetRow(int newRow)
        {
            Row = newRow;
        }

        public Cursor(char character, int row, int column) : base(character, row, column)
        {
            DisplayColumn = 0;
            DisplayRow = 0;
        }
    }
}
