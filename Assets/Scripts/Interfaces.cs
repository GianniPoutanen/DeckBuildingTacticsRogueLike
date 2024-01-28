
// Interface for undo and redo actions
using UnityEngine;

public interface IUndoRedoAction
{
    void Undo();
    void Redo();
    Entity GetEntity();
}