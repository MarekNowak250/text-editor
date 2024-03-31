using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;

namespace TextEditor
{
    internal class MoveOnDisplay
    {
        private int _maxPerRow;
        private int _maxPerColumn;
        private readonly Cursor _cursor;
        private readonly IList<List<DocumentChar>> _chars;
        private readonly MoveInMemory _moveInMemory;
        public int StartRow = 0;
        public int StartCol = 0;

        private int peekNum = 2;

        public MoveOnDisplay(Cursor cursor, IList<List<DocumentChar>> chars)
        {
            _cursor = cursor;
            _chars = chars;
            _moveInMemory = new MoveInMemory(cursor, chars);

        }

        public void Move(Direction direction, int maxPerRow, int maxPerColumn)
        {
            _maxPerColumn = maxPerColumn;
            _maxPerRow = maxPerRow;


            var oldCol = _cursor.Column;
            var oldRow = _cursor.Row;
            _moveInMemory.MoveCursor(direction);
            bool movedVertically = oldRow != _cursor.Row;
            bool movedHorizontally = oldCol != _cursor.Column;

            MoveDisplayWindowIfNecessary(movedVertically, movedHorizontally);
        }

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
            if (StartRow + _maxPerRow - peekNum < _cursor.Row)
            {
                if (StartRow + _maxPerRow + 1 == _cursor.Row)
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
            if (StartCol + _maxPerColumn - peekNum < _cursor.Column)
            {
                if (StartCol + _maxPerColumn == _cursor.Column)
                    StartCol++;
                else
                    StartCol = _cursor.Column - _maxPerColumn;
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
