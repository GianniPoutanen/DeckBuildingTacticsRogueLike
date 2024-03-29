using System.Collections.Generic;
using UnityEngine;

public class UndoRedoManager : MonoBehaviour
{
    #region Singleton Pattern
    private static UndoRedoManager instance;
    public static UndoRedoManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UndoRedoManager");
                instance = go.AddComponent<UndoRedoManager>();
            }
            return instance;
        }
    }
    #endregion Singleton Pattern

    private Stack<IUndoRedoAction> undoStack = new Stack<IUndoRedoAction>();
    private Stack<IUndoRedoAction> redoStack = new Stack<IUndoRedoAction>();

    public IUndoRedoAction NextUndo { get { return undoStack.Count > 0 ? undoStack.Peek() : null; } }
    public IUndoRedoAction NextRedo { get { return redoStack.Count > 0 ? redoStack.Peek() : null; } }

    // Method to push an action onto the undo stack
    public void Clear()
    {
        redoStack.Clear();
        undoStack.Clear();
    }


    // Method to push an action onto the undo stack
    public void AddUndoAction(IUndoRedoAction action)
    {
        undoStack.Push(action);
        redoStack.Clear(); // Clear redo stack when a new action is added
    }

    // Method to undo the last action
    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            IUndoRedoAction action = undoStack.Pop();
            action.Undo();
            redoStack.Push(action);
        }
    }

    // Method to redo the last undone action
    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            IUndoRedoAction action = redoStack.Pop();
            action.Redo();
            undoStack.Push(action);
        }
    }

    public void UndoToPlayer()
    {
        while (undoStack.Count > 0)
        {
            Ability action = undoStack.Pop() as Ability;
            if (action != null)
            {
                if (action.Performer != PlayerManager.Instance.Player && action is not CompositeAction)
                {
                    action.Undo();
                    redoStack.Push(action);
                }
                else
                {
                    action.Undo();
                    redoStack.Push(action);
                    break;
                }
            }
        }
    }
}
