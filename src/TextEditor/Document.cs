using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows.Controls;

namespace TextEditor
{
    internal class SnapshotsManager
    {
        private readonly int maxUndoSnapshots;
        private readonly int maxRedoSnapshots;
        private List<Snapshot> undoSnapshots;
        private List<Snapshot> redoSnapshots;

        public SnapshotsManager(int maxUndoSnapshots, int maxRedoSnapshots) 
        {
            undoSnapshots = new();
            redoSnapshots = new();
            this.maxUndoSnapshots = maxUndoSnapshots;
            this.maxRedoSnapshots = maxRedoSnapshots;
        }

        public void AddUndo(Snapshot snapshot, bool clear = true)
        {
            if(clear)
                redoSnapshots.Clear();

            if (undoSnapshots.Count >= maxUndoSnapshots)
                undoSnapshots.RemoveAt(0);
            undoSnapshots.Add(snapshot);
        }

        public void AddRedo(Snapshot snapshot)
        {
            if (redoSnapshots.Count >= maxRedoSnapshots)
                redoSnapshots.RemoveAt(0);
            redoSnapshots.Add(snapshot);
        }

        public Snapshot? PopUndo()
        {
            if (undoSnapshots.Count < 1)
                return null;

            int index = undoSnapshots.Count - 1;
            var snapshot = undoSnapshots[index];
            undoSnapshots.RemoveAt(index);

            AddRedo(snapshot);

            return snapshot;
        }

        public Snapshot? PopRedo()
        {
            if (redoSnapshots.Count < 1)
                return null;

            int index = redoSnapshots.Count - 1;
            var snapshot = redoSnapshots[index];
            redoSnapshots.RemoveAt(index);

            AddUndo(snapshot, false);

            return snapshot;
        }
    }

    internal class Snapshot
    {
        public readonly int CursorRow;
        public readonly int CursorColumn;
        public readonly IList<List<DocumentChar>> RowChars;
        public readonly int CharacterCount;

        public Snapshot(int cursorRow, int cursorColumn, IList<List<DocumentChar>> rowChars, int characterCount) 
        {
            CursorRow = cursorRow;
            CursorColumn = cursorColumn;
            RowChars = rowChars;
            CharacterCount = characterCount;
        }

    }

    internal class Document
    {
        public float FontSize => _charFactory.FontSize;
        public int RowCount => _editor.RowCount;
        public int CharCount => _editor.CharacterCount;

        private readonly ScrollBarDrawer _scrollBarDrawer;
        private Cursor _cursor;
        private DocumentDrawer _renderer;
        private Canvas _canvas;
        private MoveOnDisplay _mover;
        private MoveInMemory _moveInMemory;
        private CharFactory _charFactory = null;
        private ColumnUtils _columnUtils;
        private DocumentEditor _editor;

        private SnapshotsManager snapshotsManager;

        public Document(Canvas canvas, ScrollBarDrawer scrollBarDrawer)
        {
            // TODO need to replace vertical cursor moves by calcs
            // to make sure it end up around same width insead of same col num
            var rowChars = new List<List<DocumentChar>>
            {
                new List<DocumentChar>()
            };
            _editor = new DocumentEditor(rowChars, 0);
            _scrollBarDrawer = scrollBarDrawer;
            _canvas = canvas;
            _charFactory = new CharFactory(new System.Drawing.Font("Times New Roman", 12F));

            Init(_editor.RowChars);

            _canvas.SizeChanged += _canvas_SizeChanged;
        }

        private void Init(IList<List<DocumentChar>> rowChars)
        {
            _cursor = new('|', 0, 0, _charFactory.FontSize);
            _mover = new MoveOnDisplay(_cursor);
            _moveInMemory = new MoveInMemory(_cursor, rowChars);
            _columnUtils = new ColumnUtils(_charFactory, rowChars);
            if (_renderer != null)
                _renderer.Dispose();
            _renderer = new(_cursor, _canvas, rowChars, _mover, _charFactory);
            _renderer.Rerender();
            snapshotsManager = new(10, 6);
            RerenderSideBar();
        }

        private void _canvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            _renderer.Rerender();
            RerenderSideBar();
        }

        public float Zoom(int delta)
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

            return newFontSize;
        }

        public void SaveFile(string path)
        {
            new FileLoader().SaveFile(path, _editor.RowChars);
        }

        public void LoadFile(string path)
        {
            (var rowChars, int charCount) = new FileLoader().LoadFile(path);
            _editor = new DocumentEditor(rowChars, charCount);
            Init(_editor.RowChars);
            RerenderSideBar();
        }

        public void InsertChar(char character)
        {
            var row = _cursor.Row;
            var column = _cursor.Column;
            snapshotsManager.AddUndo(new Snapshot(row, column, _editor.RowChars.Select(x => new List<DocumentChar>(x)).ToList(), _editor.CharacterCount));

            if (_editor.CharacterCountInRow(row) == 0)
            {
                _editor.InsertChar(character, row, 0);
                _cursor.SetColumn(column + 1);
                _renderer.Rerender();
                return;
            }

            _editor.InsertChar(character, row, column);
            _cursor.SetColumn(column + 1);

            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount, true, true);
            _renderer.Rerender();
        }

        public void AddLine()
        {
            var row = _cursor.Row + 1;
            var charsToMove = _editor.GetRow(row - 1).Skip(_cursor.Column).ToList();

            foreach (var character in charsToMove)
            {
                _editor.RemoveChar(character, row -1);
                character.Row++;
            }

            if (row < _editor.RowCount)
                _editor.AddRow(charsToMove, row);
            else
                _editor.AddRow(charsToMove);

            _cursor.SetRow(row);
            _cursor.SetColumn(0);

            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount, true, true);
            _renderer.Rerender();
            RerenderSideBar();
        }

        public void DeleteChar()
        {
            if (_editor.RowCount == 0)
                return;
            snapshotsManager.AddRedo(new Snapshot(_cursor.Row, _cursor.Column, _editor.RowChars.Select(x => new List<DocumentChar>(x)).ToList(), _editor.CharacterCount));

            if (_cursor.Column == 0)
            {
                if (_cursor.Row < 1)
                    return;
                _cursor.SetColumn(_editor.CharacterCountInRow(_cursor.Row - 1));
                _editor.InsertRange(_editor.GetRow(_cursor.Row).ToList(), _cursor.Row -1);
                _editor.RemoveRow(_cursor.Row);
                MoveCursor(Direction.Up, true);
                return;
            }

            _editor.RemoveChar(_cursor.Row, _cursor.Column -1);
            MoveCursor(Direction.Left, true);
        }

        public void MoveCursor(Direction direction, bool textModified)
        {
            var oldCol = _cursor.Column;
            var oldRow = _cursor.Row;
            _moveInMemory.MoveCursor(direction);

            var cursorState = new CursorState(oldRow, _cursor.Row, oldCol, _cursor.Column, direction);

            int oldStartRow = _mover.StartRow;
            int oldStartCol = _mover.StartCol;

            _cursor.SetColumn(_columnUtils.GetNewColumn(cursorState, _mover.StartCol));
            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount,
                cursorState.MovedVertically(), cursorState.MovedHorizontally());

            // there is no need to rerender whole display window if it doesn't move and text hasn't changed
            // just refresh those modified rows
            if (!textModified && oldStartRow == _mover.StartRow && oldStartCol == _mover.StartCol)
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
            var nRow = (int)Math.Floor((double)_editor.RowCount * percentage / 100);
            if (nRow < 0)
                nRow = 0;
            if (nRow > _editor.RowCount)
                nRow = _editor.RowCount - 1;
            _mover.SetStartRow(nRow, _editor.RowCount);

            _renderer.Rerender();
            RerenderSideBar();
        }

        public void MoveDisplayDelta(int delta)
        {
            if (delta < 0)
                _mover.SetStartRow(_mover.StartRow + 1, _editor.RowCount);
            else
                _mover.SetStartRow(_mover.StartRow - 1, _editor.RowCount);

            _renderer.Rerender();
            RerenderSideBar();
        }

        public void MoveCursor(double x, double yPercentage)
        {
            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            int maxY = maxCount.maxRowCount;

            var row = (int)Math.Floor(maxY * yPercentage / 100);

            int realRow = _mover.StartRow + row;
            if (realRow >= _editor.RowCount)
                return;

            int col = _columnUtils.GetNewColumn(x, realRow, _mover.StartCol);

            _cursor.SetColumn(_mover.StartCol + col);
            _cursor.SetRow(realRow);

            _renderer.Rerender();
            RerenderSideBar();
        }

        public void Undo()
        {
            var snapshot = snapshotsManager.PopUndo();
            if (snapshot == null)
                return;

            RestoreFrom(snapshot);
        }

        public void Redo()
        {
            var snapshot = snapshotsManager.PopRedo();
            if (snapshot == null)
                return;

            RestoreFrom(snapshot);
        }

        private void RestoreFrom(Snapshot snapshot)
        {
            _cursor.SetColumn(snapshot.CursorColumn);
            _cursor.SetRow(snapshot.CursorRow);
            _editor = new DocumentEditor(snapshot.RowChars, snapshot.CharacterCount);
            _renderer.Rerender(snapshot.RowChars);
        }

        private void ChangeFontSize(float newSize)
        {
            _charFactory = new CharFactory(new System.Drawing.Font("Times New Roman", newSize));
            _renderer.ChangeCharFactory(_charFactory);
            _cursor.ChangeFontSize(newSize);
            _renderer.Rerender();
            _columnUtils = new ColumnUtils(_charFactory, _editor.RowChars);
        }

        private void RerenderSideBar()
        {
            _scrollBarDrawer.Rerender(_editor.RowCount, _cursor.Row, _mover.StartRow);
        }
    }
}
