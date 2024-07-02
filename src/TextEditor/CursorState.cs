﻿namespace TextEditor
{
    public struct CursorState
    {
        public readonly int PreviousRow;
        public readonly int CurrentRow;
        public readonly int PreviousColumn;
        public readonly int CurrentColumn;

        public CursorState(int previousRow, int currentRow, int previousColumn, int currentColumn)
        {
            PreviousRow = previousRow;
            CurrentRow = currentRow;
            PreviousColumn = previousColumn;
            CurrentColumn = currentColumn;
        }

        public bool MovedHorizontally() => PreviousColumn != CurrentColumn;
        public bool MovedVertically () => PreviousRow != CurrentRow;
    }
}
