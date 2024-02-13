
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Attacks/Straight Attack Action")]
public class StraightAttackAbility : Ability
{
    private enum AttackAttributes
    {
        BeforeHeatlh,
        AfterHeatlh,
        BeforeArmour,
        AfterArmour,
    }

    [HideInInspector]
    private Stack<Dictionary<GridEntity, Dictionary<AttackAttributes, int>>> attackSequanceUndoStack = new Stack<Dictionary<GridEntity, Dictionary<AttackAttributes, int>>>();
    private Stack<Dictionary<GridEntity, Dictionary<AttackAttributes, int>>> attackSequanceRedoStack = new Stack<Dictionary<GridEntity, Dictionary<AttackAttributes, int>>>();
    public int damage;
    public int range;

    public override void Undo()
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range));
        Dictionary<GridEntity, Dictionary<AttackAttributes, int>> attackSequance = attackSequanceUndoStack.Pop();
        foreach (var target in targets)
        {
            Debug.Log($"Undoing attack on {target.name}.");
            target.Health = attackSequance[target][AttackAttributes.BeforeHeatlh];
            target.Armour = attackSequance[target][AttackAttributes.BeforeArmour];
        }
        attackSequanceRedoStack.Push(attackSequance);
    }

    public override void Redo()
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(
            GridManager.Instance.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range), entityMask);
        Dictionary<GridEntity, Dictionary<AttackAttributes, int>> attackSequance = attackSequanceRedoStack.Pop();
        foreach (var target in targets)
        {
            Debug.Log($"Undoing attack on {target.name}.");
            target.Health = attackSequance[target][AttackAttributes.AfterHeatlh];
            target.Armour = attackSequance[target][AttackAttributes.AfterArmour];
        }
        attackSequanceUndoStack.Push(attackSequance);
    }
    public override void Perform()
    {
        base.Perform();
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition - Performer.targetGridPosition), range), entityMask);
        foreach (var target in targets)
        {
            Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
            Dictionary<GridEntity, Dictionary<AttackAttributes, int>> attackSequance = new Dictionary<GridEntity, Dictionary<AttackAttributes, int>>();
            attackSequance.Add(target, new Dictionary<AttackAttributes, int>());
            attackSequance[target].Add(AttackAttributes.BeforeHeatlh, target.Health);
            attackSequance[target].Add(AttackAttributes.BeforeArmour, target.Armour);
            target.Damage(damage);
            attackSequance[target].Add(AttackAttributes.AfterHeatlh, target.Health);
            attackSequance[target].Add(AttackAttributes.AfterArmour, target.Armour);
            attackSequanceUndoStack.Push(attackSequance);
        }
    }



    public override bool CanPerform(Vector3Int position)
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range), entityMask);
        return targets.Count > 0;
    }
    public override List<Vector3Int> GetAbilityPositions()
    {
        return GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition - Performer.targetGridPosition), range);
    }

    public override List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.left), range));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.up), range));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.right), range));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.down), range));
        return results;
    }
}

public class StraightAttackBuilder : AbilityBuilder
{
    StraightAttackAbility attackAbility = new StraightAttackAbility();

    public StraightAttackBuilder(StraightAttackAbility ability)
    {
        attackAbility = ability;
    }

    public override Ability Build()
    {
        return attackAbility;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        attackAbility.Performer = performer;
        return base.SetPerformer(performer);
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        attackAbility.TargetPosition = position;
        return base.SetTargetPosition(position);
    }

    public override AbilityBuilder SetDamage(int amount)
    {
        attackAbility.damage = amount;
        return base.SetDamage(amount);
    }
}