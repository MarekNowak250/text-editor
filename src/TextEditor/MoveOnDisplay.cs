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
            switch (direction)
            {
                case Direction.Right:
                    MoveRight(oldRow != _cursor.Row);
                    break;
                case Direction.Left:
                    MoveLeft(oldRow != _cursor.Row);
                    break;
                case Direction.Up:
                    MoveUp();
                    break;
                case Direction.Down:
                    MoveDown(oldRow != _cursor.Row);
                    break;
            }
        }

        public void MoveRight(bool movedVertically)
        {
            if (_maxPerColumn - 1 > _cursor.DisplayColumn + 1 &&
                StartCol + _cursor.DisplayColumn < _chars[_cursor.DisplayRow].Count - 1)
                _cursor.DisplayColumn++;
            else
            {
                if (StartCol + _cursor.DisplayColumn < _chars[_cursor.DisplayRow].Count - 1)
                    StartCol++;
                else 
                    MoveDown(movedVertically);
            }
        }

        private void MoveLeft(bool movedVertically)
        {
            if (movedVertically)
            {
                if (_cursor.DisplayRow > 0)
                    _cursor.DisplayRow--;

                _cursor.DisplayColumn = _chars[_cursor.Row].Count -1;
                StartCol = _cursor.DisplayColumn - _maxPerColumn;
                StartCol = StartCol < 0 ? 0 : StartCol;
            }
            else if (_cursor.DisplayColumn > 0)
                _cursor.DisplayColumn--;
        }

        private void MoveDown(bool movedVertically)
        {
            if (!movedVertically)
                return;

            if (_maxPerRow - 1 > _cursor.DisplayRow)
            {
                _cursor.DisplayRow++;
                _cursor.DisplayColumn = _cursor.Column;
                StartCol = _cursor.Column - _maxPerColumn;
                StartCol = StartCol < 0 ? 0 : StartCol;
            }
            else
            {
                StartRow += 1;
                _cursor.DisplayColumn = _cursor.Column;
                StartCol = _cursor.Column - _maxPerColumn;
                StartCol = StartCol < 0 ? 0 : StartCol;
            }
        }

        private void MoveUp()
        {
            if (_cursor.DisplayRow > 0)
            {
                _cursor.DisplayRow--;
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
