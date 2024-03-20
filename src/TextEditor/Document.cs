using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
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

        public Document(Canvas canvas, IList<List<DocumentChar>> rowChars = null!)
        {
            _rowChars = rowChars ?? new List<List<DocumentChar>>
            {
                new List<DocumentChar>()
            };
            _cursor = new('|', 0, 0);
            var factory = new CharFactory(new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular));

            _rowChars = new FileLoader().LoadFile(@"C:\Users\Marek.Nowak\OneDrive - Sumitomo Wiring Systems\Desktop\log.txt");
            _canvas = canvas;
            _mover = new MoveOnDisplay(_cursor, _rowChars);
            _renderer = new(factory, _cursor, canvas, _rowChars, _mover);
            //var text = File.ReadAllLines("C:\\Users\\Marek.Nowak\\Desktop\\log.txt");
            //for (int i = 0; i < text.Length; i++)
            //{
            //    var line = new List<DocumentChar>();
            //    foreach (var c in text[i])
            //    {
            //        line.Add(new DocumentChar(c, i, line.Count));
            //    }
            //    _rowChars.Add(line);
            //}
        }

        public IList<List<DocumentChar>> GetChars => _rowChars.AsReadOnly();

        public void InsertChar(char character)
        {
            var row = _cursor.Row;
            var column = _cursor.Column;
            if (_rowChars[row].Count == 0)
            {
                _rowChars[row].Add(new DocumentChar(character, row, 0));
                _renderer.Rerender();
                return;
            }

            _rowChars[row].Insert(column + 1, new DocumentChar(character, row, column + 1));
            _cursor.SetColumn(column + 1);

            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(maxCount.maxRowCount, maxCount.maxColumnCount, true, true);
            _renderer.Rerender();
        }

        public void AddLine()
        {
            var row = _cursor.Row + 1;
            _rowChars.Add(new());
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

            _rowChars[_cursor.Row].RemoveAt(_cursor.Column);
            MoveCursor(Direction.Left);
        }

        public void MoveCursor(Direction direction)
        {
            var maxCount = _renderer.GetMaxRowColCount(_canvas);
            _mover.Move(direction, maxCount.maxRowCount, maxCount.maxColumnCount);

            _renderer.Rerender();
        }
    }
}
