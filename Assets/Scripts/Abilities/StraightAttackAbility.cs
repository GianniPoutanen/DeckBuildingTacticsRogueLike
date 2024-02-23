
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Attacks/Straight Attack Action")]
public class StraightAttackAbility : Ability
{
    private int beforePerformerHealth;
    private int afterPerformerHealth;

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
    public int deadRange;
    public bool allowDiagonals;
    public bool pierce = false;
    public bool drain = false;
    public bool backstab = false;

    public override void Undo()
    {
        if (attackSequanceUndoStack.Count > 0)
        {
            Dictionary<GridEntity, Dictionary<AttackAttributes, int>> attackSequance = attackSequanceUndoStack.Pop();
            foreach (var target in attackSequance.Keys)
            {
                Debug.Log($"Undoing attack on {target.name}.");
                target.Health = attackSequance[target][AttackAttributes.BeforeHeatlh];
                target.Armour = attackSequance[target][AttackAttributes.BeforeArmour];
            }
            attackSequanceRedoStack.Push(attackSequance);
        }

        if (drain)
            Performer.CurrentHeatlh = beforePerformerHealth;
    }

    public override void Redo()
    {
        base.Redo();
        if (attackSequanceRedoStack.Count > 0)
        {
            List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(
                GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition), range), entityMask);
            Dictionary<GridEntity, Dictionary<AttackAttributes, int>> attackSequance = attackSequanceRedoStack.Pop();
            foreach (var target in targets)
            {
                Debug.Log($"Undoing attack on {target.name}.");
                target.Health = attackSequance[target][AttackAttributes.AfterHeatlh];
                target.Armour = attackSequance[target][AttackAttributes.AfterArmour];
            }
            attackSequanceUndoStack.Push(attackSequance);
        }
    }

    public override void Perform()
    {
        base.Perform();
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition - Performer.targetGridPosition), range), entityMask);
        Dictionary<GridEntity, Dictionary<AttackAttributes, int>> attackSequance = new Dictionary<GridEntity, Dictionary<AttackAttributes, int>>();
        beforePerformerHealth = Performer.Health;
        foreach (var target in targets)
        {
            Debug.Log($"{Performer.name} attacks {target.name} for {damage} damage.");
            attackSequance.Add(target, new Dictionary<AttackAttributes, int>());
            attackSequance[target].Add(AttackAttributes.BeforeHeatlh, target.Health);
            attackSequance[target].Add(AttackAttributes.BeforeArmour, target.Armour);

            if (pierce)
                target.PierceDamage(damage + Performer.GetStatus(Status.Strength));
            else
                target.Damage(damage + Performer.GetStatus(Status.Strength));

            if (drain)
            {
                Performer.Heal(attackSequance[target][AttackAttributes.BeforeHeatlh] - target.Health);
            }

            target.Damage(damage + Performer.GetStatus(Status.Strength));
            attackSequance[target].Add(AttackAttributes.AfterHeatlh, target.Health);
            attackSequance[target].Add(AttackAttributes.AfterArmour, target.Armour);
        }
        attackSequanceUndoStack.Push(attackSequance);
        afterPerformerHealth = Performer.Health;
        UIManager.Instance.UpdateUI();
    }

    public override bool CanPerform()
    {
        List<GridEntity> targets = GridManager.Instance.GetEntitiesOnPositions(this.GetPossiblePositions(Performer.targetGridPosition), entityMask);
        return targets.Count > 0;
    }
    public override List<Vector3Int> GetAbilityPositions()
    {
        return GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(TargetPosition - Performer.targetGridPosition), range, deadRange);
    }

    public override List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.left), range, deadRange));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.up), range, deadRange));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.right), range, deadRange));
        results.AddRange(GridManager.Instance.GetPositionsInDirection(Performer.targetGridPosition, GridManager.RoundToCardinal(Vector3Int.down), range, deadRange));
        return results;
    }

    public override void HighlightPossiblePositions()
    {
        base.HighlightPossiblePositions();
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

    public override AbilityBuilder SetAmount(int amount)
    {
        attackAbility.damage = amount;
        return base.SetAmount(amount);
    }
}