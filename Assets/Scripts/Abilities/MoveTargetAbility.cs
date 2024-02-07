
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Movement/Move Action")]
public class MoveTargetAbility : Ability
{
	private Vector3Int oldPosition;
	private Vector3Int newPosition;
    [HideInInspector]
	public Vector3Int targetPosition;
    public GridEntity target;
	public int distance;


	public override void Undo()
	{
		_performer.transform.position = oldPosition;
		_performer.targetGridPosition = oldPosition;
	}

	public override void Redo()
	{
		_performer.transform.position = newPosition;
		_performer.targetGridPosition = newPosition;
	}

	public override void Perform()
	{
		base.Perform();
		Debug.Log("Moving " + _performer + " from " + oldPosition + " to " + newPosition);
		oldPosition = _performer.targetGridPosition;
		target.targetGridPosition = targetPosition * new Vector3Int(1, 1, 0);
		newPosition = targetPosition;
	}

	public override bool CanPerform(Vector3Int position)
	{
		return (_performer.targetGridPosition * new Vector3Int(1, 1, 0)).Equals(position) &&
			_performer.CanMoveTo(position) && GridManager.Instance.FindPath(_performer.targetGridPosition, position).Count > distance + 2 && GridManager.Instance.GetEntityOnPosition(position) == null;
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
		moveSelfAbility.targetPosition = position;
		return this;
	}

	public override AbilityBuilder SetRange(int range)
	{
		moveSelfAbility.distance = range;
		return this;
	}
}