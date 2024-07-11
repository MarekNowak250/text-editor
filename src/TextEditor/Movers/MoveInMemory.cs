using System.Collections.Generic;
using System.Linq;
using TextEditor.Enums;

namespace TextEditor
{
    public class MoveInMemory
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
        }

        private void MoveRight(IList<List<DocumentChar>> chars)
        {
            if (_cursor.Column <  chars[_cursor.Row].Count())
                _cursor.SetColumn(_cursor.Column + 1);
            else if (_cursor.Row + 1 < chars.Count())
            {
                _cursor.SetColumn(0);
                _cursor.SetRow(_cursor.Row + 1);
            }
        }

        private void MoveLeft(IList<List<DocumentChar>> chars)
        {
            if (_cursor.Column > 0)
                _cursor.SetColumn(_cursor.Column - 1);
            else if (_cursor.Row > 0)
            {
                _cursor.SetRow(_cursor.Row - 1);
                _cursor.SetColumn(chars[_cursor.Row].Count());
            }
        }

        private void MoveDown(IList<List<DocumentChar>> chars)
        {
            if (chars.Count() - 1 > _cursor.Row)
            {
                _cursor.SetRow(_cursor.Row + 1);
                if (chars[_cursor.Row].Count() < _cursor.Column)
                {
                    var newCol = chars[_cursor.Row].Count();
                    _cursor.SetColumn(newCol);
                }
            }
        }

        private void MoveUp(IList<List<DocumentChar>> chars)
        {
            if (_cursor.Row > 0)
            {
                _cursor.SetRow(_cursor.Row - 1);
                if (chars[_cursor.Row].Count() < _cursor.Column)
                {
                    var newCol = chars[_cursor.Row].Count();
                    _cursor.SetColumn(newCol);
                }

            }
        }
    }
}
