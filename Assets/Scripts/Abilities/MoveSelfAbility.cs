
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Movement/Move Self Action")]
public class MoveSelfAbility : Ability
{
    private Vector3Int oldPosition;
    private Vector3Int newPosition;
    [HideInInspector]
    public int range;

    private int prevPlayerNumMovement;
    private int newPlayerNumMovement;

    public MoveSelfAbility()
    {
    }

    public MoveSelfAbility(MoveSelfAbility ability) : base(ability)
    {
        this.range = ability.range;
    }

    public override void Undo()
    {
        Performer.transform.position = oldPosition;
        Performer.targetGridPosition = oldPosition;

        if (Performer == PlayerManager.Instance.Player)
        {
            PlayerManager.Instance.numMovement = prevPlayerNumMovement;

            if (PlayerManager.Instance.numMovement > PlayerManager.Instance.maxMovement)
                PlayerManager.Instance.numMovement = PlayerManager.Instance.maxMovement;
        }
    }

    public override void Redo()
    {
        Performer.transform.position = newPosition;
        Performer.targetGridPosition = newPosition;

        if (Performer == PlayerManager.Instance.Player)
        {
            PlayerManager.Instance.numMovement = newPlayerNumMovement;

            if (PlayerManager.Instance.numMovement <= 0)
                PlayerManager.Instance.numMovement = 0;
        }

    }

    public override void Perform()
    {
        base.Perform();
        Debug.Log("Moving " + Performer + " from " + oldPosition + " to " + newPosition);
        oldPosition = Performer.targetGridPosition;

        if (Performer == PlayerManager.Instance.Player)
        {
            prevPlayerNumMovement = PlayerManager.Instance.numMovement;
        }

        Performer.targetGridPosition = TargetPosition * new Vector3Int(1, 1, 0);
        newPosition = TargetPosition;

        if (Performer == PlayerManager.Instance.Player)
        {
            newPlayerNumMovement = PlayerManager.Instance.numMovement;
        }

    }

    public override bool CanPerform(Vector3Int position)
    {
        if (GridManager.Instance.GetEntityOnPosition(position) != null)
            return false;
        int travelPathCount = GridManager.Instance.FindPath(Performer.targetGridPosition, position).Count;
        return Performer.CanMoveTo(position) && travelPathCount != 0 && travelPathCount <= range;
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
        moveSelfAbility.TargetPosition = position;
        return this;
    }

    public override AbilityBuilder SetRange(int range)
    {
        moveSelfAbility.range = range;
        return this;
    }
}