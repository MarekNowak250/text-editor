using FluentAssertions;

namespace TextEditor.Tests.Unit.MoveOnDisplayTests
{
    public class MoveRightTests
    {
        private readonly MoveOnDisplay _sut;
        private readonly Cursor _cursor;
        private readonly IList<List<DocumentChar>> _chars;

        public MoveRightTests()
        {
            _cursor = new Cursor('|', 0, 0);
            _chars = new List<List<DocumentChar>>
            {
                new List<DocumentChar>(){new DocumentChar(',', 0, 0) }
            };

            _sut = new(_cursor, _chars);
        }

        [Fact]
        public void MoveRight_Should_do_nothing_When_there_is_no_next_row_or_next_char()
        {
            _sut.Move(Direction.Right, 10, 10);

            _cursor.DisplayColumn.Should().Be(0);
            _cursor.DisplayRow.Should().Be(0);
        }

        [Fact]
        public void MoveRight_Should_change_cursor_column_When_there_is_char_next_to_it()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _sut.Move(Direction.Right, 10, 10);

            _cursor.DisplayColumn.Should().Be(1);
            _cursor.DisplayRow.Should().Be(0);
        }

        [Fact]
        public void MoveRight_Should_move_to_next_row_When_there_is_no_next_char()
        {
            _chars.Add(new List<DocumentChar>() { new DocumentChar('1', 1, 0) });
            _sut.Move(Direction.Right, 10, 10);

            _cursor.DisplayColumn.Should().Be(0);
            _cursor.DisplayRow.Should().Be(1);
        }

        [Fact]
        public void MoveRight_Should_move_display_window_When_it_exceded_max_display_chars_in_column()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _sut.Move(Direction.Right, 1, 1);

            _cursor.DisplayColumn.Should().Be(0);
            _cursor.DisplayRow.Should().Be(0);
            _sut.StartCol.Should().Be(1);
        }
    }
}
