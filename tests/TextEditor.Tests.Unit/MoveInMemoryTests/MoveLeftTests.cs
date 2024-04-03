using FluentAssertions;
using System.Collections;

namespace TextEditor.Tests.Unit.MoveInMemoryTests
{

    public class MoveLeftTests : MoveInMemoryTest
    {
        public MoveLeftTests() : base() { }

        [Fact]
        public void MoveLeft_Should_do_nothing_When_there_is_no_previous_char_and_no_row_above()
        {
            _sut.MoveCursor(Direction.Left);

            _cursor.Row.Should().Be(0);
            _cursor.Column.Should().Be(0);
        }

        [Fact]
        public void MoveLeft_Should_go_to_previous_char_When_there_is_previous_char()
        {
            _chars[0].Add(new DocumentChar(',', 0, 1));
            _cursor.SetColumn(1);
            _sut.MoveCursor(Direction.Left);

            _cursor.Row.Should().Be(0);
            _cursor.Column.Should().Be(0);
        }

        [Theory]
        [ClassData(typeof(MoveLeftDataGenerator))]
        public void MoveLeft_Should_go_to_previous_line_When_there_in_no_previous_char_and_there_is_line_above
            (IList<List<DocumentChar>> chars, Cursor cursor, int expectedRow, 
            int expectedColumn)
        {
            var sut = new MoveInMemory(cursor, chars);
            sut.MoveCursor(Direction.Left);

            cursor.Row.Should().Be(expectedRow);
            cursor.Column.Should().Be(expectedColumn);
        }

        public class MoveLeftDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
        {
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1) },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1), new DocumentChar(',', 0, 2) }
                   },
                new Cursor('|', 1, 0),
                0, 2
            },
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1), new DocumentChar(',', 0, 2)  },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1) }
                   },
                new Cursor('|', 1, 0),
                0, 3
            }
        };

            public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
