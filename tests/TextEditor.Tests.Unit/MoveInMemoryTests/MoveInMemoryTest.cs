namespace TextEditor.Tests.Unit.MoveInMemoryTests
{
    public abstract class MoveInMemoryTest
    {
        protected readonly MoveInMemory _sut;
        protected readonly Cursor _cursor;
        protected readonly IList<List<DocumentChar>> _chars;

        public MoveInMemoryTest()
        {
            _cursor = new Cursor('|', 0, 0, 12);
            _chars = new List<List<DocumentChar>>
            {
                new List<DocumentChar>(){new DocumentChar(',', 0, 0) }
            };

            _sut = new(_cursor, _chars);
        }

    }
}
