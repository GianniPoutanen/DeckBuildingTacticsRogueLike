
// Interface for undo and redo actions
using System;
using UnityEngine;

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}

public interface IUndoRedoAction
{
    GridEntity Performer { get; set; }
    void Undo();
    void Redo();
    void Perform(Vector3Int position);
}
