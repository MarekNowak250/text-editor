using System.Collections.Generic;

namespace TextEditor
{
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
}
