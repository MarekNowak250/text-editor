using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Tests.Unit.SnapshotsManager.Tests
{
    public class RedoTests
    {
        [Fact]
        public void AddRedo_ShouldRemoveFirstItem_WhenMaxCapacityIsReached()
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var testSnapshot2 = new Snapshot(1, 1, new List<List<DocumentChar>>(), 1);

            var sut = new TextEditor.SnapshotsManager(1, 1);
            sut.AddRedo(testSnapshot);
            
            sut.AddRedo(testSnapshot2);

            var redo = sut.PopRedo();
            var redo2 = sut.PopRedo();

            redo.Should().BeEquivalentTo(testSnapshot2);
            redo.Should().NotBeEquivalentTo(testSnapshot);
            redo2.Should().BeNull();
        }

        [Fact]
        public void PopRedo_ShouldReturnNull_WhenThereIsNoSnapshots()
        {
            var sut = new TextEditor.SnapshotsManager(1, 1);
            
            var red = sut.PopRedo();

            red.Should().BeNull();
        }

        [Fact]
        public void PopRedo_ShouldReturnLastAddedItem_WhenThereIsAny()
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var testSnapshot2 = new Snapshot(1, 1, new List<List<DocumentChar>>(), 1);

            var sut = new TextEditor.SnapshotsManager(2, 2);
            sut.AddRedo(testSnapshot);

            sut.AddRedo(testSnapshot2);

            var redo = sut.PopRedo();
            var redo2 = sut.PopRedo();
            var redo3 = sut.PopRedo();

            redo.Should().BeEquivalentTo(testSnapshot2);
            redo.Should().NotBeEquivalentTo(testSnapshot);
            redo2.Should().BeEquivalentTo(testSnapshot);
            redo3.Should().BeNull();
        }

        [Fact]
        public void PopRedo_ShouldAddPoppedSnapshotToUndo()
        {
            var testSnapshot = new Snapshot(0, 0, new List<List<DocumentChar>>(), 0);
            var sut = new TextEditor.SnapshotsManager(2, 2);    
            sut.AddRedo(testSnapshot);

            sut.PopRedo();

            var undo = sut.PopUndo();
            undo.Should().BeEquivalentTo(testSnapshot);
        }
    }
}
