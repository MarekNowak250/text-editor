using FluentAssertions;
using System.Collections;

namespace TextEditor.Tests.Unit.MoveInMemoryTests
{
    public class MoveRightTests : MoveInMemoryTest
    {
        public MoveRightTests() : base() { }

        [Fact]
        public void MoveRight_Should_do_nothing_When_there_is_no_next_char_and_no_row_below()
        {
            _sut.MoveCursor(Direction.Right);

            _cursor.Row.Should().Be(0);
            _cursor.Column.Should().Be(0);
        }

        [Fact]
        public void MoveRight_Should_go_to_next_char_When_there_is_next_char()
        {
            _chars[0].Add(new DocumentChar(',', 0, 1));
            _sut.MoveCursor(Direction.Right);

            _cursor.Row.Should().Be(0);
            _cursor.Column.Should().Be(1);
        }

        [Theory]
        [ClassData(typeof(MoveRightDataGenerator))]
        internal void MoveRight_Should_go_to_next_line_When_there_in_no_next_char_and_there_is_line_below
            (IList<List<DocumentChar>> chars, Cursor cursor, int expectedRow, 
            int expectedColumn)
        {
            var sut = new MoveInMemory(cursor, chars);
            sut.MoveCursor(Direction.Right);

            cursor.Row.Should().Be(expectedRow);
            cursor.Column.Should().Be(expectedColumn);
        }
    }

    public class MoveRightDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1) },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1), new DocumentChar(',', 1, 2) }
                   },
                new Cursor('|', 0, 1),
                1,1
            },
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1) },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1) }
                   },
                new Cursor('|', 0, 1),
                1,1
            },
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1) },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0) }
                   },
                new Cursor('|', 0, 1),
                1,0
            },
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
