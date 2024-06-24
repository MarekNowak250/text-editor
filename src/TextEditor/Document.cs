using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TextEditor
{
    internal class Document
    {
        public IList<List<DocumentChar>> GetChars => _rowChars.AsReadOnly();
        private IList<List<DocumentChar>> _rowChars;
        private readonly ScrollBarDrawer _scrollBarDrawer;
        private Cursor _cursor;
        private DocumentDrawer _renderer;
        private Canvas _canvas;
        private MoveOnDisplay _mover;
        private MoveInMemory _moveInMemory;
        object insertLock = new();

        public Document(Canvas canvas, ScrollBarDrawer scrollBarDrawer, IList<List<DocumentChar>> rowChars = null!)
        {
            _rowChars = rowChars ?? new List<List<DocumentChar>>
            {
                new List<DocumentChar>()
            };
            _scrollBarDrawer = scrollBarDrawer;
            _canvas = canvas;

            Init(_rowChars);
            _canvas.SizeChanged += _canvas_SizeChanged;
        }

        private void Init(IList<List<DocumentChar>> _rowChars)
        {
            _cursor = new('|', 0, 0);
            _mover = new MoveOnDisplay(_cursor);
            _moveInMemory = new MoveInMemory(_cursor, _rowChars);
            if (_renderer != null)
                _renderer.Dispose();
            _renderer = new(_cursor, _canvas, _rowChars, _mover);
            _renderer.Rerender();
        }

        private void _canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _renderer.Rerender();
            RerenderSideBar();
        }

        public void SaveFile(string path)
        {
            new FileLoader().SaveFile(path, _rowChars);
        }

        public void LoadFile(string path)
        {
            _rowChars = new FileLoader().LoadFile(path);
            Init(_rowChars);
            RerenderSideBar();
        }

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
            RerenderSideBar();
        }

        public void DeleteChar()
        {
            if (_rowChars.Count == 0 )
                return;

            if (_cursor.Column == 0)
            {
                if (_cursor.Row < 1)
                    return;
                _cursor.SetColumn(_rowChars[_cursor.Row - 1].Count);
                _rowChars[_cursor.Row - 1].AddRange(_rowChars[_cursor.Row]);
                _rowChars.RemoveAt(_cursor.Row);
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
            RerenderSideBar();
        }

        public void MoveDisplay(int percentage)
        {
            var nRow = (int)Math.Floor((double)_rowChars.Count * percentage / 100);
            if (nRow < 0)
                nRow = 0;
            if(nRow > _rowChars.Count)
                nRow = _rowChars.Count -1;
            _mover.StartRow = nRow;

            _renderer.Rerender();
            RerenderSideBar();
        }

        private void RerenderSideBar()
        {
            _scrollBarDrawer.Rerender(_rowChars.Count, _cursor.Row, _mover.StartRow);
        }
    }
}
