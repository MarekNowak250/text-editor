using System.Collections.Generic;

namespace TextEditor
{
    internal class SnapshotsManager
    {
        private readonly int maxUndoSnapshots;
        private readonly int maxRedoSnapshots;
        private List<Snapshot> undoSnapshots;
        private List<Snapshot> redoSnapshots;

        public SnapshotsManager(int maxUndoSnapshots, int maxRedoSnapshots) 
        {
            undoSnapshots = new();
            redoSnapshots = new();
            this.maxUndoSnapshots = maxUndoSnapshots;
            this.maxRedoSnapshots = maxRedoSnapshots;
        }

        public void AddUndo(Snapshot snapshot, bool clear = true)
        {
            if(clear)
                redoSnapshots.Clear();

            if (undoSnapshots.Count >= maxUndoSnapshots)
                undoSnapshots.RemoveAt(0);
            undoSnapshots.Add(snapshot);
        }

        public void AddRedo(Snapshot snapshot)
        {
            if (redoSnapshots.Count >= maxRedoSnapshots)
                redoSnapshots.RemoveAt(0);
            redoSnapshots.Add(snapshot);
        }

        public Snapshot? PopUndo()
        {
            if (undoSnapshots.Count < 1)
                return null;

            int index = undoSnapshots.Count - 1;
            var snapshot = undoSnapshots[index];
            undoSnapshots.RemoveAt(index);

            AddRedo(snapshot);

            return snapshot;
        }

        public Snapshot? PopRedo()
        {
            if (redoSnapshots.Count < 1)
                return null;

            int index = redoSnapshots.Count - 1;
            var snapshot = redoSnapshots[index];
            redoSnapshots.RemoveAt(index);

            AddUndo(snapshot, false);

            return snapshot;
        }
    }
}
