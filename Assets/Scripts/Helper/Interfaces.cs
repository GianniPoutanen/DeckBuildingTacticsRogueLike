
// Interface for undo and redo actions
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}

public interface IUndoRedoAction
{
    void Undo();
    void Redo();
    void Perform();
}

public interface IAbilityBuilder
{
    Ability Build();
}

public interface IEnemyAttack
{
    public bool CanPerform(Vector3 position);
    public void ActionAttack();
    public List<Vector3Int> GetEffectivePositions();
}