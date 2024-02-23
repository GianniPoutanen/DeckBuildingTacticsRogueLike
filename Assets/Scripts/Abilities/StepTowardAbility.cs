
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Abilities/Movement/Step Toward")]
public class StepTowardAbility : Ability
{
    private Vector3Int oldPosition;
    private Vector3Int newPosition;
    [Header("Stats")]
    public int triggerRange;
    public int jumpDistance;

    public bool forceStep;


    public override void Undo()
    {
        base.Undo();
        Performer.transform.position = oldPosition;
        Performer.targetGridPosition = oldPosition;
    }

    public override void Redo()
    {
        base.Redo();
        Performer.transform.position = newPosition;
        Performer.targetGridPosition = newPosition;
    }

    public override void Perform()
    {
        base.Perform();
        Debug.Log("Moving " + Performer + " from " + oldPosition + " to " + newPosition);
        oldPosition = Performer.targetGridPosition;
        Performer.targetGridPosition = GetPositionOfStep();
        newPosition = Performer.targetGridPosition;
    }

    private Vector3Int GetPositionOfStep()
    {
        List<Vector3Int> possibleStepPositions = new List<Vector3Int>();

        Vector3Int targetDirection = (GridManager.RoundToCardinal(TargetPosition - Performer.targetGridPosition));
        int range = (Vector3Int.Distance(Performer.targetGridPosition, TargetPosition) - 1) > jumpDistance ? jumpDistance : (int)(Vector3Int.Distance(Performer.targetGridPosition, TargetPosition) - 1);

        possibleStepPositions = GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, targetDirection, jumpDistance);
        Vector3Int closestStep = Performer.targetGridPosition;

        foreach (var possibleStep in possibleStepPositions)
        {
            if (Vector3Int.Distance(Performer.targetGridPosition, TargetPosition) > Vector3Int.Distance(Performer.targetGridPosition, possibleStep) && 
                Vector3Int.Distance(Performer.targetGridPosition, possibleStep) > Vector3Int.Distance(Performer.targetGridPosition, closestStep) && 
                !GridManager.Instance.IsEntityOnPosition(possibleStep))
            {
                closestStep = possibleStep;
            }
        }
        return closestStep;
    }

    public override bool CanPerform()
    {
        return Vector3Int.Distance(TargetPosition, Performer.targetGridPosition) <= triggerRange && (GetPositionOfStep() != Performer.targetGridPosition || forceStep);
    }

    public override List<Vector3Int> GetPossiblePositions()
    {
        return GetPossiblePositions(Performer.targetGridPosition);
    }

    public override List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.left), triggerRange));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.up), triggerRange));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.right), triggerRange));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.down), triggerRange));
        return results;
    }

    public override void HighlightPossiblePositions()
    {
        base.HighlightPossiblePositions();
        GridManager.Instance.HighlightSelectedPositions(GetJumpPossiblePositions(), TileMapType.EnemyMovePositions, TileType.EnemyMoveTile);
    }


    public List<Vector3Int> GetJumpPossiblePositions()
    {
        List<Vector3Int> results = new List<Vector3Int>();
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.left), jumpDistance));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.up), jumpDistance));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.right), jumpDistance));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.down), jumpDistance));
        return results;
    }
}

public class StepTowardBuilder : AbilityBuilder
{
    StepTowardAbility moveAwayBuilder;

    public StepTowardBuilder(StepTowardAbility ability)
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
        moveAwayBuilder.triggerRange = range;
        return this;
    }
}