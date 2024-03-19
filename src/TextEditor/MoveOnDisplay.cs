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
            switch (direction)
            {
                case Direction.Right:
                case Direction.Left:
                    MoveDisplayWindowIfNecessary(movedVertically, movedHorizontally); 
                    break;
                case Direction.Up:
                    MoveUp();
                    break;
                case Direction.Down:
                    MoveDown(oldRow != _cursor.Row);
                    break;
            }
        }

        public void MoveDisplayWindowIfNecessary(bool movedVertically, bool movedHorizontally)
        {
            if (movedVertically)
            {
                if (StartRow + _maxPerRow < _cursor.Row && StartRow + 1 < _chars.Count - 1)
                {
                    StartRow++;
                }
                if (StartRow > _cursor.Row && StartRow - 1 >= 0)
                {
                    StartRow--;
                }
            }
            if (movedHorizontally)
            {
                if (StartCol + _maxPerColumn < _cursor.Column 
                    && StartCol + 1 < _chars[_cursor.Row].Count - 1)
                {
                    StartCol++;
                }
                if (StartCol > _cursor.Column && StartCol - 1 >= 0)
                {
                    StartCol--;
                }
            }
        }

        private void MoveDown(bool movedVertically)
        {
            if (!movedVertically)
                return;

            //if (StartRow < _chars.Count - 1)
            //{
            //    StartRow += 1;
            //    StartCol = _cursor.Column - _maxPerColumn;
            //    StartCol = StartCol < 0 ? 0 : StartCol;
            //}
        }

        private void MoveUp()
        {
            if (true)//_cursor.DisplayRow > 0)
            {
                //_cursor.DisplayRow--;
                //_cursor.DisplayColumn = _cursor.Column;
                //StartCol = _cursor.Column - _maxPerColumn;
                //StartCol = StartCol < 0 ? 0 : StartCol;
            }
            else
            {
                if (StartRow > 0)
                    StartRow -= 1;
            }
        }
    }
}
