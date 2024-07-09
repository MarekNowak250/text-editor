using System.Collections.Generic;
using System.Linq;

namespace TextEditor
{
    internal class DocumentEditor
    {
        public int CharacterCount { get; private set; }
        public int RowCount => _rowChars.Count;
        public IList<List<DocumentChar>> RowChars => _rowChars.AsReadOnly();
        public int CharacterCountInRow(int row) => _rowChars[row].Count;
        public IReadOnlyCollection<DocumentChar> GetRow(int row) => _rowChars[row].AsReadOnly();

        private readonly IList<List<DocumentChar>> _rowChars;

        public DocumentEditor(IList<List<DocumentChar>> rowChars, int characterCount)
        {
            _rowChars = rowChars;
            CharacterCount = characterCount;
        }

        public void InsertChar(char character, int row, int column)
        {
            _rowChars[row].Insert(column, new DocumentChar(character, row, column));

            CharacterCount++;
        }

        public void InsertRange(IList<DocumentChar> characters, int row, int index = -1)
        {
            if (index != -1)
                _rowChars[row].InsertRange(index, characters);
            else
                _rowChars[row].AddRange(characters);

            CharacterCount += characters.Count;
        }

        public void AddRow(IList<DocumentChar> characters, int index = -1)
        {
            if (index != -1)
                _rowChars.Insert(index, characters.ToList());
            else
                _rowChars.Add(characters.ToList());

            CharacterCount += characters.Count;
        }

        public void RemoveRow(int row)
        {
            CharacterCount -= _rowChars[row].Count; 

            _rowChars.RemoveAt(row);
        }

        public void RemoveChar(DocumentChar character, int row)
        {
            _rowChars[row].Remove(character);
            CharacterCount--;
        }

        public void RemoveChar(int row, int column)
        {
            _rowChars[row].RemoveAt(column);
            CharacterCount--;
        }
    }
}
