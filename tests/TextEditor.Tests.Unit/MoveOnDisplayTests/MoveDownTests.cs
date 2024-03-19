//using FluentAssertions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TextEditor.Tests.Unit.MoveOnDisplayTests
//{
//    public class MoveDownTests
//    {
//        private readonly MoveOnDisplay _sut;
//        private readonly Cursor _cursor;
//        private readonly IList<List<DocumentChar>> _chars;

//        public MoveDownTests()
//        {
//            _cursor = new Cursor('|', 0, 0);
//            _chars = new List<List<DocumentChar>>
//            {
//                new List<DocumentChar>(){new DocumentChar(',', 0, 0) }
//            };

//            _sut = new(_cursor, _chars);
//        }

//        [Fact]
//        public void MoveDown_Should_do_nothing_When_there_is_no_row_below()
//        {
//            _sut.Move(Direction.Down, 10, 10);

//            _cursor.DisplayColumn.Should().Be(0);
//            _cursor.DisplayRow.Should().Be(0);
//        }

//        [Fact]
//        public void MoveDown_Should_change_cursor_row_When_there_row_below()
//        {
//            _chars[0].Add(new DocumentChar('1', 0, 1));
//            _sut.Move(Direction.Left, 10, 10);

//            _cursor.DisplayColumn.Should().Be(0);
//            _cursor.DisplayRow.Should().Be(1);
//        }


//        [Fact]
//        public void MoveLeft_Should_move_to_previous_row_When_there_is_previous_char()
//        {
//            _chars.Add(new List<DocumentChar>() { new DocumentChar('1', 1, 0) });
//            _cursor.SetRow(1);
//            _cursor.DisplayRow = 1;
//            _sut.Move(Direction.Left, 10, 10);

//            _cursor.DisplayColumn.Should().Be(0);
//            _cursor.DisplayRow.Should().Be(0);
//            _sut.StartCol.Should().Be(0);
//        }

//        [Fact]
//        public void MoveLeft_Should_does_not_change_displ_row_When_it_is_already_on_0()
//        {
//            _chars[0].Add(new DocumentChar('1', 0, 1));
//            _sut.Move(Direction.Left, 1, 1);

//            _cursor.DisplayColumn.Should().Be(0);
//            _cursor.DisplayRow.Should().Be(0);
//            _sut.StartCol.Should().Be(0);
//        }
//    }
//}
