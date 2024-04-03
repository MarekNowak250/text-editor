using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Tests.Unit.MoveInMemoryTests
{
    public class MoveDownTests: MoveInMemoryTest
    {
        public MoveDownTests() : base() { }

        [Fact]
        public void MoveDown_Should_do_nothing_When_there_is_no_row_below()
        {
            _sut.MoveCursor(Direction.Down);

            _cursor.Column.Should().Be(0);
            _cursor.Column.Should().Be(0);
        }

        [Theory]
        [ClassData(typeof(MoveDownDataGenerator))]
        public void MoveDown_Should_move_cursor_down_When_there_is_row_below
             (IList<List<DocumentChar>> chars, Cursor cursor, int expectedRow, 
                int expectedColumn)
        {
            _cursor.SetColumn(1);
            var sut = new MoveInMemory(cursor, chars);
            sut.MoveCursor(Direction.Down);

            cursor.Row.Should().Be(expectedRow);
            cursor.Column.Should().Be(expectedColumn);
        }
    }

    public class MoveDownDataGenerator : IEnumerable<object[]>
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
                new Cursor('|', 0, 0),
                1, 0
            },
            new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1), new DocumentChar(',', 0, 2)  },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1) }
                   },
                new Cursor('|', 0, 1),
                1, 1
            },
                        new object[]
            {
                new List<List<DocumentChar>>
                    {
                    new List<DocumentChar>(){new DocumentChar(',', 0, 0), new DocumentChar(',', 0, 1), new DocumentChar(',', 0, 2)  },
                    new List<DocumentChar>(){new DocumentChar(',', 1, 0), new DocumentChar(',', 1, 1) }
                   },
                new Cursor('|', 0, 2),
                1, 1
            }
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
