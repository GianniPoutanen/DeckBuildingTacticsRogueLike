
// Interface for undo and redo actions
using System;
using UnityEngine;

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}
public interface IUndoRedoAction
{
    void Undo();
    void Redo();
    Entity GetEntity();
}


public interface IAttackAction : IUndoRedoAction
{
    void ExecuteAttack();
}

public interface IHealAction : IUndoRedoAction
{
    void ExecuteHeal();
}

public interface IUseAbilityAction : IUndoRedoAction
{
    void ExecuteAbility();
}
