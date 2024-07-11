using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Tests.Unit.SnapshotsManager.Tests
{
    public class UndoTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddUndo_ShouldClearRedoSnapshots_WhenClearIsTrue(bool clear)
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var sut = new TextEditor.SnapshotsManager(2, 2);
            sut.AddRedo(testSnapshot);

            sut.AddUndo(testSnapshot, clear);

            var redo = sut.PopRedo();

            if (clear)
                redo.Should().BeNull();
            else
                redo.Should().NotBeNull();
        }

        [Fact]
        public void AddUndo_ShouldRemoveFirstItem_WhenMaxCapacityIsReached()
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var testSnapshot2 = new Snapshot(1, 1, new List<List<DocumentChar>>(), 1);

            var sut = new TextEditor.SnapshotsManager(1, 1);
            sut.AddUndo(testSnapshot);
            
            sut.AddUndo(testSnapshot2);

            var undo = sut.PopUndo();
            var undo2 = sut.PopUndo();

            undo.Should().BeEquivalentTo(testSnapshot2);
            undo.Should().NotBeEquivalentTo(testSnapshot);
            undo2.Should().BeNull();
        }

        [Fact]
        public void PopUndo_ShouldReturnNull_WhenThereIsNoSnapshots()
        {
            var sut = new TextEditor.SnapshotsManager(1, 1);
            
            var undo = sut.PopUndo();

            undo.Should().BeNull();
        }

        [Fact]
        public void PopUndo_ShouldReturnLastAddedItem_WhenThereIsAny()
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var testSnapshot2 = new Snapshot(1, 1, new List<List<DocumentChar>>(), 1);

            var sut = new TextEditor.SnapshotsManager(2, 2);
            sut.AddUndo(testSnapshot);

            sut.AddUndo(testSnapshot2);

            var undo = sut.PopUndo();
            var undo2 = sut.PopUndo();
            var undo3 = sut.PopUndo();

            undo.Should().BeEquivalentTo(testSnapshot2);
            undo.Should().NotBeEquivalentTo(testSnapshot);
            undo2.Should().BeEquivalentTo(testSnapshot);
            undo3.Should().BeNull();
        }

        [Fact]
        public void PopUndo_ShouldAddPoppedSnapshotToRedo()
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var sut = new TextEditor.SnapshotsManager(2, 2);    
            sut.AddUndo(testSnapshot);

            sut.PopUndo();

            var redo = sut.PopRedo();
            redo.Should().BeEquivalentTo(testSnapshot);
        }
    }
}
