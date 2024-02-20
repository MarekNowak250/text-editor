using System.Collections.Generic;
using System.Linq;

namespace TextEditor
{
    internal class MoveInMemory
    {
        private readonly Cursor _cursor;
        private readonly IList<List<DocumentChar>> chars;

        public MoveInMemory(Cursor cursor, IList<List<DocumentChar>> chars)
        {
            _cursor = cursor;
            this.chars = chars;
        }

        public void MoveCursor(Direction direction)
        {
            int oldRow = _cursor.Row;
            int oldColumn = _cursor.Column;
            switch (direction)
            {
                case Direction.Right:
                    MoveRight(chars);
                    break;
                case Direction.Left:
                    MoveLeft(chars);
                    break;
                case Direction.Up:
                    MoveUp(chars);
                    break;
                case Direction.Down:
                    MoveDown(chars);
                    break;
            }
            chars[oldRow].RemoveAt(oldColumn);
            chars[_cursor.Row].Insert(_cursor.Column, _cursor);
        }

        public void MoveRight(IList<List<DocumentChar>> chars)
        {
            if (chars[_cursor.Row].Count() - 1 > _cursor.Column)
                _cursor.SetColumn(_cursor.Column+1);
            else if (chars.Count() - 1 > _cursor.Row)
            {
                _cursor.SetColumn(0);
                _cursor.SetRow(_cursor.Row + 1);
            }
        }

        public void MoveLeft(IList<List<DocumentChar>> chars)
        {
            if (_cursor.Column > 0)
                _cursor.SetColumn(_cursor.Column - 1);
            else if (_cursor.Row > 0)
            {
                _cursor.SetRow(_cursor.Row - 1);
                _cursor.SetColumn(chars[_cursor.Row].Count()-1);
            }
        }

        public void MoveDown(IList<List<DocumentChar>> chars)
        {
            if (chars.Count() - 1 > _cursor.Row)
            {
                _cursor.SetRow(_cursor.Row + 1);
                if (chars[_cursor.Row].Count() - 1 < _cursor.Column)
                    _cursor.SetColumn(chars[_cursor.Row].Count());
            }
        }

        public void MoveUp(IList<List<DocumentChar>> chars)
        {
            if (_cursor.Row > 0)
            {
                _cursor.SetRow(_cursor.Row-1);
                if (chars[_cursor.Row].Count() - 1 < _cursor.Column)
                    _cursor.SetColumn(chars[_cursor.Row].Count());
            }
        }
    }
}
