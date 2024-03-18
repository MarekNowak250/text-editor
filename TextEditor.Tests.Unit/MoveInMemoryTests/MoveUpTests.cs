using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextEditor.Tests.Unit.MoveInMemoryTests.MoveLeftTests;

namespace TextEditor.Tests.Unit.MoveInMemoryTests
{
    public class MoveUpTests: MoveInMemoryTest
    {
        public MoveUpTests() : base() { }

        [Fact]
        public void MoveUp_Should_do_nothing_When_there_is_no_row_above()
        {
            _sut.MoveCursor(Direction.Up);

            _cursor.Column.Should().Be(0);
            _cursor.Column.Should().Be(0);
        }

        [Theory]
        [ClassData(typeof(MoveUpDataGenerator))]
        public void MoveUp_Should_move_cursor_up_When_there_is_row_above
            (IList<List<DocumentChar>> chars, Cursor cursor, int expectedRow, 
                int expectedColumn)
        {
            var sut = new MoveInMemory(cursor, chars);
            sut.MoveCursor(Direction.Up);

            cursor.Row.Should().Be(expectedRow);
            cursor.Column.Should().Be(expectedColumn);
        }
    }

    public class MoveUpDataGenerator : IEnumerable<object[]>
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
                0, 0
            },
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1), new DocumentChar(',', 0, 2)  },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1) }
                   },
                new Cursor('|', 1, 1),
                0, 1
            },
                        new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1)  },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1), new DocumentChar(',', 1, 2) }
                   },
                new Cursor('|', 1, 2),
                0, 1
            }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
