using System.Collections.Generic;
using System.Linq;

namespace TextEditor
{
    public enum Direction
    {
        Right,
        Left, 
        Up, 
        Down
    }
    internal class Cursor : DocumentChar
    {
        public Cursor(char character, int row, int column) : base(character, row, column)
        {
        }

        public void MoveCursor(Direction direction, Dictionary<int, List<DocumentChar>> chars)
        {
            int oldRow = Row;
            int oldColumn = Column;
            switch (direction)
            {
                case Direction.Right:
                    MoveRight(chars);
                    break;
                case Direction.Left:
                    MoveLeft(chars);
                    break;
                case Direction.Up:
                    MoveUp(chars);
                    break;
                case Direction.Down:
                    MoveDown(chars);
                    break;
            }
            chars[oldRow].RemoveAt(oldColumn);
            chars[Row].Insert(Column, this);
        }

        public void MoveRight(Dictionary<int, List<DocumentChar>> chars)
        {
            if (chars[Row].Count() - 1 > Column)
                Column++;
            else if (chars.Count() - 1 > Row)
            {
                Column = 0;
                Row++;
            }
        }

        public void MoveLeft(Dictionary<int, List<DocumentChar>> chars)
        {
            if (Column > 0)
                Column--;
            else if (Row > 0)
            {
                Row--;
                Column = chars[Row].Count() -1;
            }
        }

        public void MoveDown(Dictionary<int, List<DocumentChar>> chars)
        {
            if (chars.Count() - 1 > Row)
            {
                Row++;
                if (chars[Row].Count() - 1 < Column)
                    Column = chars[Row].Count();
            }
        }

        public void MoveUp(Dictionary<int, List<DocumentChar>> chars)
        {
            if (Row > 0)
            {
                Row--;
                if (chars[Row].Count() - 1 < Column)
                    Column = chars[Row].Count();
            }
        }
    }
}
