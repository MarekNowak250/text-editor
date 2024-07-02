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
        private CharFactory _charFactory = null;
        private ColumnUtils _columnUtils;

        public Document(Canvas canvas, ScrollBarDrawer scrollBarDrawer, IList<List<DocumentChar>> rowChars = null!)
        {
            // TODO need to replace vertical cursor moves by calcs
            // to make sure it end up around same width insead of same col num
            _rowChars = rowChars ?? new List<List<DocumentChar>>
            {
                new List<DocumentChar>()
            };
            _scrollBarDrawer = scrollBarDrawer;
            _canvas = canvas;
            _charFactory = new CharFactory(new System.Drawing.Font("Courier New", 12F));

            Init(_rowChars);
            _canvas.SizeChanged += _canvas_SizeChanged;
        }

        private void Init(IList<List<DocumentChar>> _rowChars)
        {
            _cursor = new('|', 0, 0, _charFactory.FontSize);
            _mover = new MoveOnDisplay(_cursor);
            _moveInMemory = new MoveInMemory(_cursor, _rowChars);
            _columnUtils = new ColumnUtils(_charFactory, _rowChars);
            if (_renderer != null)
                _renderer.Dispose();
            _renderer = new(_cursor, _canvas, _rowChars, _mover, _charFactory);
            _renderer.Rerender();
            RerenderSideBar();
        }

        private void _canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _renderer.Rerender();
            RerenderSideBar();
        }

        public void Zoom(int delta)
        {
            float newFontSize = _charFactory.FontSize;
            if (delta < 0)
                newFontSize -= 1.5F;
            else
                newFontSize += 1.5F;

            if (newFontSize < 1)
                newFontSize = 1;

            ChangeFontSize(newFontSize);

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

            _rowChars[row].Insert(column, new DocumentChar(character, row, column));
            _cursor.SetColumn(column + 1);


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
            if (_rowChars.Count == 0)
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

            var cursorState = new CursorState(oldRow, _cursor.Row, oldCol, _cursor.Column);

            int oldStartRow = _mover.StartRow;
            int oldStartCol = _mover.StartCol;

            _cursor.SetColumn(_columnUtils.GetNewColumn(cursorState, _mover.StartCol));
            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount,
                cursorState.MovedVertically(), cursorState.MovedHorizontally());

            // there is no need to rerender whole display window if it doesn't move just refresh those modified rows
            if (oldStartRow == _mover.StartRow && oldStartCol == _mover.StartCol)
            {
                if (cursorState.MovedVertically())
                    _renderer.RerenderRow(oldRow);
                _renderer.RerenderRow(_cursor.Row);
            }
            else
                _renderer.Rerender();
            RerenderSideBar();
        }

        public void MoveDisplay(int percentage)
        {
            var nRow = (int)Math.Floor((double)_rowChars.Count * percentage / 100);
            if (nRow < 0)
                nRow = 0;
            if (nRow > _rowChars.Count)
                nRow = _rowChars.Count - 1;
            _mover.SetStartRow(nRow, _rowChars.Count);

            _renderer.Rerender();
            RerenderSideBar();
        }

        public void MoveDisplayDelta(int delta)
        {
            if (delta < 0)
                _mover.SetStartRow(_mover.StartRow + 1, _rowChars.Count);
            else
                _mover.SetStartRow(_mover.StartRow - 1, _rowChars.Count);

            _renderer.Rerender();
            RerenderSideBar();
        }

        public void MoveCursor(double x, double yPercentage)
        {
            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            int maxY = maxCount.maxRowCount;

            var row = (int)Math.Floor(maxY * yPercentage / 100);

            int realRow = _mover.StartRow + row;
            if (realRow > _rowChars.Count)
                return;

            int col = _columnUtils.GetNewColumn(x, realRow, _mover.StartCol);

            _cursor.SetColumn(_mover.StartCol + col);
            _cursor.SetRow(realRow);

            _renderer.Rerender();
            RerenderSideBar();
        }

        private void ChangeFontSize(float newSize)
        {
            _charFactory = new CharFactory(new System.Drawing.Font("Courier New", newSize));
            _renderer.ChangeCharFactory(_charFactory);
            _cursor.ChangeFontSize(newSize);
            _renderer.Rerender();
            _columnUtils = new ColumnUtils(_charFactory, _rowChars);
        }

        private void RerenderSideBar()
        {
            _scrollBarDrawer.Rerender(_rowChars.Count, _cursor.Row, _mover.StartRow);
        }
    }
}
