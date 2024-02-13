using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject, IUndoRedoAction
{
    [HideInInspector]
    public GridEntity _performer = null;
    [HideInInspector]
    public GridEntity Performer { get { return _performer; } set { _performer = value; } }
    public Vector3Int TargetPosition { get; set; }

    [SerializeField]
    public List<string> entityMask;

    public Ability()
    {
    }

    public Ability(Ability ability)
    {
        this.Performer = ability.Performer;
        this.TargetPosition = ability.TargetPosition;
    }

    public virtual void Redo()
    {
        throw new NotImplementedException();
    }

    public virtual void Undo()
    {
        throw new NotImplementedException();
    }

    public virtual void Perform()
    {
        UndoRedoManager.Instance.AddUndoAction(this);
    }

    public virtual bool CanPerform(Vector3Int targetPosition)
    {
        TargetPosition = targetPosition;
        return CanPerform();
    }

    public virtual bool CanPerform()
    {
        return true;
    }

    public virtual List<Vector3Int> GetAbilityPositions()
    {
        return new List<Vector3Int>();
    }

    public List<Vector3Int> GetPossiblePositions()
    {
        return GetPossiblePositions(TargetPosition);
    }
    public virtual List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        return new List<Vector3Int>();
    }

    public virtual void HighlightSelectedPositions()
    {

    }
}

[System.Serializable]
public class AbilityWrapper : IUndoRedoAction
{
    public Ability ability;

    public AbilityWrapper(Ability action)
    {
        this.ability = action;
    }

    public GridEntity Performer { get { return ability.Performer; } set { ability.Performer = value; } }

    public void Perform()
    {
        ability.Perform();
    }

    public void Redo()
    {
        ability.Redo();
    }

    public void Undo()
    {
        ability.Undo();
    }
}

public abstract class AbilityBuilder : IAbilityBuilder
{
    public virtual AbilityBuilder SetPerformer(GridEntity performer)
    {
        return this;
    }

    public virtual AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        return this;
    }

    public virtual AbilityBuilder SetRange(int range)
    {
        return this;
    }

    public virtual AbilityBuilder SetCard(Card card)
    {
        return this;
    }

    public virtual AbilityBuilder SetDamage(int amount)
    {
        return this;
    }

    public virtual AbilityBuilder SetEntityMask(List<string> mask)
    {
        return this;
    }

    public virtual AbilityBuilder SetCanPierce(bool canPierce)
    {
        return this;
    }

    public static AbilityBuilder GetBuilder(Ability ability)
    {
        switch (ability)
        {
            case DestroyEntityAbility:
                return new DestroyEntityBuilder((DestroyEntityAbility) ability);
            case MoveSelfAbility:
                return new MoveSelfBuilder(new MoveSelfAbility((MoveSelfAbility)ability));
            case MoveTargetAbility:
                return new MoveTargetBuilder((MoveTargetAbility)ability);
            case SimpleAttackAbility:
                return new SimpleAttackBuilder(new SimpleAttackAbility((SimpleAttackAbility)ability));
            case StraightAttackAbility:
                return new StraightAttackBuilder((StraightAttackAbility)ability);
            case CompositeAction:
                return new CompositeAbilityBuilder((CompositeAction)ability);
        }
        return null;
    }

    public abstract Ability Build();
}