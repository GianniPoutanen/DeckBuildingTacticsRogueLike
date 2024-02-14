
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Abilities/Attacks/Attack Action")]
public class SimpleAttackAbility : Ability
{
    [HideInInspector]
    private int beforeHealth;
    private int afterHealth;
    private int beforeArmour;
    private int afterArmour;
    public int damage;
    [HideInInspector]
    public bool global;
    public int range;

    public SimpleAttackAbility() 
    {

    }

    public SimpleAttackAbility(SimpleAttackAbility ability) : base(ability)
    {
        range = ability.range;
        global = ability.global;
        damage = ability.damage;
    }

    public override void Undo()
    {
        base.Undo();
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        Debug.Log($"Undoing attack on {target.name}.");
        if (target != null)
            target.Health = beforeHealth;
    }

    public override void Redo()
    {
        base.Redo();
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        Debug.Log($"Redoing attack on {target.name}.");
        if (target != null)
            target.Health = afterHealth;
    }
    public override void Perform()
    {
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        base.Perform();
        if (target != null)
        {
            Debug.Log($"{Performer.name} attacks {target.name} for {damage} damage.");
            beforeArmour = target.Armour;
            beforeHealth = target.Health;
            target.Damage(damage);
            afterArmour = target.Armour;
            afterHealth = target.Health;
            UIManager.Instance.UpdateUI();
        }
    }

    public override bool CanPerform(Vector3Int position)
    {
        var target = GridManager.Instance.GetEntityOnPosition(position, entityMask);
        return target != null && target != Performer && (global || GridManager.Instance.GetWalkingDistance(Performer.targetGridPosition, position) <= range);
    }

    public override List<Vector3Int> GetAbilityPositions()
    {
        return new List<Vector3Int> { TargetPosition };
    }

    public override List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        if (!global)
            foreach (Vector3Int pos in GridManager.Instance.GetGridPositionsWithinDistance(originPosition, range))
                if (GridManager.Instance.GetWalkingDistance(originPosition, pos) <= range && !pos.Equals(Performer.targetGridPosition))
                    results.Add(pos);
        return results;
    }
}


public class SimpleAttackBuilder : AbilityBuilder
{
    SimpleAttackAbility attackAbility;

    public SimpleAttackBuilder()
    {
        attackAbility = new SimpleAttackAbility();
    }

    public SimpleAttackBuilder(SimpleAttackAbility attackAbility)
    {
        this.attackAbility = attackAbility;
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

    public override AbilityBuilder SetRange(int range)
    {
        attackAbility.range = range;
        attackAbility.global = false;
        return this;
    }
}