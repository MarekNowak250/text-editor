using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TextEditor
{
    internal class Document
    {
        private Dictionary<int, List<DocumentChar>> _rowChars;
        private Cursor _cursor;

        public Document(Dictionary<int, List<DocumentChar>> rowChars = null)
        {
            _rowChars = rowChars ?? new Dictionary<int, List<DocumentChar>>
            {
                { 0, new List<DocumentChar>() }
            };
            _cursor = new('|', 0, 0);
            _rowChars[0].Add(_cursor);
        }

        public ReadOnlyDictionary<int, List<DocumentChar>> GetChars => _rowChars.AsReadOnly();

        public void InsertChar(char character)
        {
            var row = _cursor.Row;
            var column = _cursor.Column;
            _rowChars[row].Insert(column, new DocumentChar(character, row, column));
            _cursor.MoveRight(_rowChars);
        }

        public void AddLine()
        {
            _rowChars.Add(_rowChars.Count, new());
            _cursor.MoveCursor(Direction.Down, _rowChars);
        }

        public void DeleteChar()
        {
            if (_cursor.Column == 0)
                return;
            _cursor.MoveCursor(Direction.Left, _rowChars);
            _rowChars[_cursor.Row].RemoveAt(_cursor.Column + 1);
        }

        public void MoveCursor(Direction direction)
        {
            _cursor.MoveCursor(direction, _rowChars);
        }
    }
}
