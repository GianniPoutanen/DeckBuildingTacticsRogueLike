
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attacks/Straight Attack Action")]
public class StraightAttackAbility : Ability
{
    [HideInInspector]
    private Dictionary<GridEntity, int> beforeHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> beforeArmour = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterArmour = new Dictionary<GridEntity, int>();
    public int damage;
    public int range;

    public override void Undo()
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range));
        foreach (var target in targets)
        {
            Debug.Log($"Undoing attack on {target.name}.");
            target.Health = beforeHealth[target];
        }
    }

    public override void Redo()
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(
            GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range), entityMask);
        foreach (var target in targets)
        {
            Debug.Log($"Redoing attack on {target.name}.");
            target.Health = afterHealth[target];
        }
    }
    public override void Perform()
    {
        base.Perform();
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range), entityMask);
        foreach (var target in targets)
        {
            Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
            beforeHealth.Add(target, target.Health);
            beforeArmour.Add(target, target.Armour);
            target.Damage(damage);
            afterHealth.Add(target, target.Health);
            afterArmour.Add(target, target.Armour);
        }
    }

    public override bool CanPerform(Vector3Int position)
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range), entityMask);
        return targets.Count > 0;
    }

    public override List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        results.AddRange(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.left), range));
        results.AddRange(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.up), range));
        results.AddRange(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.right), range));
        results.AddRange(GridManager.GetPositionsInDirection(_performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.down), range));
        return base.GetPossiblePositions(originPosition);
    }
}

public class StraightAttackBuilder : AbilityBuilder
{
    StraightAttackAbility straightAttackAbility = new StraightAttackAbility();

    public StraightAttackBuilder(StraightAttackAbility ability)
    {
        straightAttackAbility = ability;
    }

    public override Ability Build()
    {
        return straightAttackAbility;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        straightAttackAbility.Performer = performer;
        return SetPerformer(performer);
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        straightAttackAbility.TargetPosition = position;
        return SetTargetPosition(position);
    }

    public override AbilityBuilder SetDamage(int amount)
    {
        straightAttackAbility.damage = amount;
        return base.SetDamage(amount);
    }
}