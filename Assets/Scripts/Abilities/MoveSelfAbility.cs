
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Movement/Move Self Action")]
public class MoveSelfAbility : Ability
{
    private Vector3Int oldPosition;
    private Vector3Int newPosition;
    [HideInInspector]
    public Vector3Int targetPosition;
    public int range;

    public override void Undo()
    {
        _performer.transform.position = oldPosition;
        _performer.targetGridPosition = oldPosition;

        if (_performer == PlayerManager.Instance.Player)
            PlayerManager.Instance.numMovement++;

        if (PlayerManager.Instance.numMovement > PlayerManager.Instance.maxMovement)
            PlayerManager.Instance.numMovement = PlayerManager.Instance.maxMovement;
    }

    public override void Redo()
    {
        _performer.transform.position = newPosition;
        _performer.targetGridPosition = newPosition;

        if (_performer == PlayerManager.Instance.Player)
            PlayerManager.Instance.numMovement--;

        if(PlayerManager.Instance.numMovement <= 0)
            PlayerManager.Instance.numMovement = 0;
    }

    public override void Perform()
    {
        base.Perform();
        Debug.Log("Moving " + _performer + " from " + oldPosition + " to " + newPosition);
        oldPosition = _performer.targetGridPosition;
        _performer.targetGridPosition = targetPosition * new Vector3Int(1, 1, 0);
        newPosition = targetPosition;
    }

    public override bool CanPerform(Vector3Int position)
    {
        if (GridManager.Instance.GetEntityOnPosition(position) != null)
            return false;
        int travelPathCount = GridManager.Instance.FindPath(_performer.targetGridPosition, position).Count;
        return _performer.CanMoveTo(position) && travelPathCount != 0 && travelPathCount <= range;
    }

    public Ability Build()
    {
        throw new NotImplementedException();
    }
}

public class MoveSelfBuilder : AbilityBuilder  
{
    MoveSelfAbility moveSelfAbility;

    public override Ability Build()
    {
        return moveSelfAbility;
    }
    public MoveSelfBuilder()
    {
        moveSelfAbility = new MoveSelfAbility(); ;
    }

    public MoveSelfBuilder(MoveSelfAbility ability)
    {
        moveSelfAbility = ability;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        moveSelfAbility.Performer = performer;
        return this;
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        moveSelfAbility.targetPosition = position;
        return this;
    }

    public override AbilityBuilder SetRange(int range)
    {
        moveSelfAbility.range = range;
        return this;
    }
}