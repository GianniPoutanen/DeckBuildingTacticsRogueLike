
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Movement/Move Action")]
public class MoveTargetAbility : Ability
{
	private Vector3Int oldPosition;
	private Vector3Int newPosition;
    [HideInInspector]
    public GridEntity target;
	public int distance;


	public override void Undo()
	{
		Performer.transform.position = oldPosition;
		Performer.targetGridPosition = oldPosition;
	}

	public override void Redo()
	{
		Performer.transform.position = newPosition;
		Performer.targetGridPosition = newPosition;
	}

	public override void Perform()
	{
		base.Perform();
		Debug.Log("Moving " + Performer + " from " + oldPosition + " to " + newPosition);
		oldPosition = Performer.targetGridPosition;
		target.targetGridPosition = TargetPosition * new Vector3Int(1, 1, 0);
		newPosition = TargetPosition;
	}

	public override bool CanPerform(Vector3Int position)
	{
		return (Performer.targetGridPosition * new Vector3Int(1, 1, 0)).Equals(position) &&
			Performer.CanMoveTo(position) && GridManager.Instance.FindPath(Performer.targetGridPosition, position).Count > distance + 2 && GridManager.Instance.GetEntityOnPosition(position) == null;
	}
}

public class MoveTargetBuilder : AbilityBuilder
{
	MoveTargetAbility moveSelfAbility;

	public MoveTargetBuilder(MoveTargetAbility ability)
	{
		this.moveSelfAbility = ability;
	}

	public override Ability Build()
	{
		return moveSelfAbility;
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
		moveSelfAbility.distance = range;
		return this;
	}
}