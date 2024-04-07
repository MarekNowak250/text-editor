using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TextEditor
{
    internal class Document
    {
        private IList<List<DocumentChar>> _rowChars;
        private Cursor _cursor;
        private Renderer _renderer;
        private Canvas _canvas;
        private MoveOnDisplay _mover;
        private readonly MoveInMemory _moveInMemory;

        public Document(Canvas canvas, IList<List<DocumentChar>> rowChars = null!)
        {
            _rowChars = rowChars ?? new List<List<DocumentChar>>
            {
                new List<DocumentChar>()
            };
            _cursor = new('|', 0, 0);
            var factory = new CharFactory(new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular));

            _rowChars = new FileLoader().LoadFile(@"C:\Users\marek\Desktop\kmp\test.txt");
            _canvas = canvas;
            _mover = new MoveOnDisplay(_cursor);
            _moveInMemory = new MoveInMemory(_cursor, _rowChars);
            _renderer = new(factory, _cursor, canvas, _rowChars, _mover);
            _renderer.Rerender();

            _canvas.SizeChanged += _canvas_SizeChanged;
        }

        private void _canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _renderer.Rerender();
        }

        public IList<List<DocumentChar>> GetChars => _rowChars.AsReadOnly();

        object insertLock = new();

        public void InsertChar(char character)
        {
            var row = _cursor.Row;
            var column = _cursor.Column;
            if (_rowChars[row].Count == 0)
            {
                _rowChars[row].Add(new DocumentChar(character, row, 0));
                _renderer.Rerender();
                _cursor.SetColumn(column + 1);
                return;
            }

            lock (insertLock)
            {
                _rowChars[row].Insert(column, new DocumentChar(character, row, column));
                _cursor.SetColumn(column + 1);
            }

            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount, true, true);
            _renderer.Rerender();
        }

        public void AddLine()
        {
            var row = _cursor.Row + 1;
            var charsToMove = _rowChars[row - 1].Skip(_cursor.Column).ToList();

            foreach (var character in charsToMove)
            {
                _rowChars[row - 1].Remove(character);
                character.Row++;
            }

            if (row < _rowChars.Count)
                _rowChars.Insert(row, charsToMove);
            else
                _rowChars.Add(charsToMove);

            _cursor.SetRow(row);
            _cursor.SetColumn(0);

            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount, true, true);
            _renderer.Rerender();
        }

        public void DeleteChar()
        {
            if (_rowChars.Count == 0 || _rowChars[_cursor.Row].Count == 0)
                return;

            if (_cursor.Column == 0)
            {
                _cursor.SetColumn(_rowChars[_cursor.Row - 1].Count);
                _rowChars[_cursor.Row - 1].AddRange(_rowChars[_cursor.Row]);
                _rowChars[_cursor.Row] = new();
                MoveCursor(Direction.Up);
                return;
            }

            _rowChars[_cursor.Row].RemoveAt(_cursor.Column - 1);
            MoveCursor(Direction.Left);
        }

        public void MoveCursor(Direction direction)
        {
            var oldCol = _cursor.Column;
            var oldRow = _cursor.Row;
            _moveInMemory.MoveCursor(direction);
            bool movedVertically = oldRow != _cursor.Row;
            bool movedHorizontally = oldCol != _cursor.Column;

            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount, 
                movedVertically, movedHorizontally);

            _renderer.Rerender();
        }
    }
}
