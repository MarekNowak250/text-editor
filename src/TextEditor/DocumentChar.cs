namespace TextEditor
{
    public class DocumentChar
    {
        public readonly char Character;
        public int Row { get; protected set; }
        public int Column { get; protected set; }

        public DocumentChar(char character, int row, int column)
        {
            Character = character;
            Row = row;
            Column = column;
        }
    }
}
