using System.Collections.Generic;
using System.Linq;

namespace TextEditor
{
    public class ColumnUtils
    {
        private readonly CharFactory _charFactory;
        private readonly IList<List<DocumentChar>> _chars;

        public ColumnUtils(CharFactory charFactory, IList<List<DocumentChar>> chars)
        {
            _charFactory = charFactory;
            _chars = chars;
        }

        public int GetNewColumn(double maxWidth, int row, int startCol)
        {
            int col = 0;

            double colWidth = 0;
            foreach (var c in _chars[row].Skip(startCol))
            {
                colWidth += _charFactory.GetCharRender(c.Character).Width;
                if (colWidth > maxWidth)
                    break;
                col++;
            }

            return col;
        }

        public int GetNewColumn(CursorState cursorState, int startCol)
        {
            if (!cursorState.MovedVertically())
                return cursorState.CurrentColumn; 

            double previousWidth = 0;
            foreach (var c in _chars[cursorState.PreviousRow].Skip(startCol).Take(cursorState.PreviousColumn - startCol)) 
            { 
                previousWidth += _charFactory.GetCharRender(c.Character).Width;
            }

            return GetNewColumn(previousWidth, cursorState.CurrentRow, startCol);
        }
    }
}
