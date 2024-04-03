using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Tests.Unit.MoveOnDisplayTests
{
    public class MoveLeftTests
    {
        private readonly MoveOnDisplay _sut;
        private readonly Cursor _cursor;
        private readonly IList<List<DocumentChar>> _chars;

        public MoveLeftTests()
        {
            _cursor = new Cursor('|', 0, 0);
            _chars = new List<List<DocumentChar>>
            {
                new List<DocumentChar>(){new DocumentChar(',', 0, 0) }
            };

            _sut = new(_cursor, _chars, 0);
        }

        [Fact]
        public void MoveLeft_Should_move_display_window_When_moved_cursor_is_no_longer_visible()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _cursor.SetColumn(1);
            _sut.Move(Direction.Left, 1, 1);

            _sut.StartCol.Should().Be(0);
        }

        [Fact]
        public void MoveLeft_Should_move_display_window_When_cursor_moved_up_outside_of_displ_window()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _chars.Add(new() { new DocumentChar('1', 1, 0), new DocumentChar('1', 1, 1), });
            _cursor.SetRow(1);

            _sut.Move(Direction.Left, 1, 1);

            _sut.StartRow.Should().Be(0);
            _sut.StartCol.Should().Be(1);
        }

        [Fact]
        public void MoveLeft_Should_move_display_window_When_cursor_moved_up_outside_of_displ_window2()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _chars.Add(new() { new DocumentChar('1', 1, 0), new DocumentChar('1', 1, 1), });
            _cursor.SetRow(1);

            _sut.Move(Direction.Left, 1, 2);

            _sut.StartRow.Should().Be(0);
            _sut.StartCol.Should().Be(0);
        }
    }
}
