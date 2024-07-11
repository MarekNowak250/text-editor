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
            _cursor = new Cursor('|', 0, 0, 12);
            _chars = new List<List<DocumentChar>>
            {
                new List<DocumentChar>(){new DocumentChar(',', 0, 0) }
            };

            _sut = new(_cursor);
        }

        [Fact]
        public void MoveRight_Should_move_display_window_When_it_exceded_max_display_chars_in_column()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _sut.Move(1, 1, false, false);

            _sut.StartCol.Should().Be(1);
        }

        [Fact]
        public void MoveRight_Should_move_display_window_When_it_exceded_max_display_rows()
        {
            _chars[0].Add(new DocumentChar('1', 0, 1));
            _chars.Add(new() { new DocumentChar('1', 1, 0), new DocumentChar('1', 1, 1), });
            _cursor.SetColumn(1);

            _sut.Move(1, 1, false, false);

            _sut.StartRow.Should().Be(1);
            _sut.StartCol.Should().Be(0);
        }
    }
}
