using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace TextEditor
{
    internal interface IDisplayWindow
    {
        public int GetStartRow();
        public int GetStartCol();
    }

    internal class MoveOnDisplay: IDisplayWindow
    {
        public int StartRow = 0;
        public int StartCol = 0;
        private int _maxPerRow;
        private int _maxPerColumn;
        private readonly Cursor _cursor;
        private readonly int _peekNum;

        public MoveOnDisplay(Cursor cursor, 
            int peekNum =2)
        {
            _cursor = cursor;
            _peekNum = peekNum;
        }

        public int GetStartRow() => StartRow;
        public int GetStartCol() => StartCol;


        public void Move(int maxPerRow, int maxPerColumn, 
            bool movedVertically, bool movedHorizontally)
        {
            _maxPerColumn = maxPerColumn;
            _maxPerRow = maxPerRow;

            MoveDisplayWindowIfNecessary(movedVertically, movedHorizontally);
        }

        public void MoveDisplayWindowIfNecessary(bool movedVertically, bool movedHorizontally)
        {
            if (movedVertically)
                MoveVertically();
            if (movedHorizontally)
                MoveHorizontally();
        }

        private void MoveVertically()
        {
            if (StartRow + _maxPerRow - _peekNum < _cursor.Row)
            {
                if (StartRow + _maxPerRow - _peekNum + 1 == _cursor.Row)
                    StartRow++;
                else
                    StartRow = _cursor.Row;
            }
            if (StartRow > _cursor.Row && StartRow - 1 >= 0)
            {
                if (StartRow - 1 == _cursor.Row)
                    StartRow--;
                else
                    StartRow = _cursor.Row;
            }
        }

        private void MoveHorizontally()
        {
            if (StartCol + _maxPerColumn - _peekNum - 1 < _cursor.Column)
            {
                if (StartCol + _maxPerColumn + 1 == _cursor.Column)
                    StartCol++;
                else
                    StartCol = _cursor.Column - _maxPerColumn + 1;
                if (StartCol < 0)
                    StartCol = 0;
            }
            if (StartCol > _cursor.Column && StartCol - 1 >= 0)
            {
                if (StartCol - 1 == _cursor.Column)
                    StartCol--;
                else
                    StartCol = _cursor.Column;
            }
        }
    }
}
