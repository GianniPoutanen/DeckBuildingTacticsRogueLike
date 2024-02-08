
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Abilities/Movement/Move Action")]
public class StepAwayAbility : Ability
{
	private Vector3Int oldPosition;
	private Vector3Int newPosition;
    [HideInInspector]
    public GridEntity target;
	public int range;
	public int distance;

	public bool forceStep;


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
		target.targetGridPosition = GetPositionOfStep();
        newPosition = TargetPosition;
	}

    private Vector3Int GetPositionOfStep()
    {
		List<Vector3Int> possibleStepPositions = new List<Vector3Int>();

		Vector3Int targetDirection = (GridManager.RoundToCardinal(TargetPosition - Performer.targetGridPosition) * (-Vector3Int.one));
		possibleStepPositions = GridManager.Instance.GetPositionsInStraightLine(Performer.targetGridPosition, new Vector2(targetDirection.x, targetDirection.y), distance);
        return possibleStepPositions.OrderBy(p => Vector3.Distance(p, TargetPosition)).First();
    }

    public override bool CanPerform(Vector3Int position)
	{
		return GetPositionOfStep() != null || forceStep;
	}
}

public class StepAwayBuilder : AbilityBuilder
{
	StepAwayAbility moveAwayBuilder;

	public StepAwayBuilder(StepAwayAbility ability)
	{
		this.moveAwayBuilder = ability;
	}

	public override Ability Build()
	{
		return moveAwayBuilder;
	}

	public override AbilityBuilder SetPerformer(GridEntity performer)
	{
		moveAwayBuilder.Performer = performer;
		return this;
	}

	public override AbilityBuilder SetTargetPosition(Vector3Int position)
	{
		moveAwayBuilder.TargetPosition = position;
		return this;
	}

	public override AbilityBuilder SetRange(int range)
	{
		moveAwayBuilder.range = range;
		return this;
	}
}