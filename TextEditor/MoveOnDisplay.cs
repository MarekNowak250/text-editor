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

            _moveInMemory.MoveCursor(direction);
            switch (direction)
            {
                case Direction.Right:
                    MoveRight();
                    break;
                case Direction.Left:
                    MoveLeft();
                    break;
                case Direction.Up:
                    MoveUp();
                    break;
                case Direction.Down:
                    MoveDown();
                    break;
            }
        }

        public void MoveRight()
        {
            if (_maxPerColumn - 1 > _cursor.DisplayColumn +1 && 
                StartCol + _cursor.DisplayColumn < _chars[_cursor.DisplayRow].Count - 1)
                _cursor.DisplayColumn++;
            else
            {
                if(StartCol + _cursor.DisplayColumn +1 < _chars[_cursor.DisplayRow].Count - 1)
                    StartCol++;
                else
                {
                    MoveDown();
                }
            }
        }

        public void MoveLeft()
        {
            if (_cursor.DisplayColumn > 0)
                _cursor.DisplayColumn--;
            else if (_cursor.DisplayRow > 0)
            {
                _cursor.DisplayRow--;
                _cursor.DisplayColumn = _cursor.Column;
                StartCol = _cursor.Column - _maxPerColumn;
            }
        }

        public void MoveDown()
        {
            if (_maxPerRow - 2 > _cursor.DisplayRow)
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

        public void MoveUp()
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
                StartRow -= 1;
            }
        }
    }
}
