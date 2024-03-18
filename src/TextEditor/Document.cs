using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace TextEditor
{
    internal class Document
    {
        private IList<List<DocumentChar>> _rowChars;
        private Cursor _cursor;
        private Renderer _renderer;

        public Document(Canvas canvas, IList<List<DocumentChar>> rowChars = null)
        {
            _rowChars = rowChars ?? new List<List<DocumentChar>>
            {
                new List<DocumentChar>()
            };
            _cursor = new('|', 0, 0);
            _rowChars[0].Add(new DocumentChar(' ', 0, 0));
            var factory = new CharFactory(new System.Drawing.Font("Helvetica", 12F));
            _renderer = new(factory, _cursor, canvas, _rowChars);

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
            _rowChars[row].Insert(column, new DocumentChar(character, row, column));
            _renderer.Rerender(Direction.Right);
        }

        public void AddLine()
        {
            _rowChars.Add(new());
            _renderer.Rerender(Direction.Down);
        }

        public void DeleteChar()
        {
            if (_cursor.Column == 0)
                return;
            _rowChars[_cursor.Row].RemoveAt(_cursor.Column);
            _renderer.Rerender(Direction.Left);
        }

        public void MoveCursor(Direction direction)
        {
            _renderer.Rerender(direction);
        }
    }
}
