
using System.Collections.Generic;
using UnityEngine;

public class CompositeAction : IUndoRedoAction
{
    private List<IUndoRedoAction> actions = new List<IUndoRedoAction>();
    private Entity entity;
    public CompositeAction(List<IUndoRedoAction> actions, Entity entity)
    {
        this.actions = actions;
        this.entity = entity;
    }

    public void Undo()
    {
        for (int i = 0; i <= actions.Count - 1; i++)
        {
            actions[i].Undo();
        }
    }

    public void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Redo();
        }
    }

    public Entity GetEntity()
    {
        return entity;
    }
}


/// <summary>
/// GRID ENTITY ACTIONS
/// </summary>
public class MoveGridEntity : IUndoRedoAction
{
    private GridEntity gridEntity;
    private Vector3Int oldPosition;
    private Vector3Int newPosition;
    private Entity entity;

    public MoveGridEntity(GridEntity entity, Vector3Int oldPosition, Vector3Int newPosition)
    {
        this.gridEntity = entity;
        this.oldPosition = oldPosition;
        this.newPosition = newPosition;
    }

    public void Undo()
    {
        gridEntity.transform.position = oldPosition;
        gridEntity.targetGridPosition = oldPosition;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public void Redo()
    {
        gridEntity.transform.position = newPosition;
        gridEntity.targetGridPosition = newPosition;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }
    public Entity GetEntity()
    {
        return gridEntity;
    }
}