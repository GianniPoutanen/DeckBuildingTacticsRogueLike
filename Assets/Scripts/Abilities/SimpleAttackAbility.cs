
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
    private int beforePerformerHealth;
    private int afterPerformerHealth;
    private int beforeHealth;
    private int afterHealth;
    private int beforeArmour;
    private int afterArmour;
    public int damage;
    [HideInInspector]
    public bool global { get { return range == 0; } }
    [Tooltip("If 0 then ability effect will be global")]
    public int range;
    public bool pierce = false;
    public bool drain = false;

    public SimpleAttackAbility()
    {

    }

    public SimpleAttackAbility(SimpleAttackAbility ability) : base(ability)
    {
        range = ability.range;
        damage = ability.damage;
    }

    public override void Undo()
    {
        base.Undo();
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        Debug.Log($"Undoing attack on {target.name}.");
        if (target != null)
            target.Health = beforeHealth;

        if (drain)
            Performer.CurrentHeatlh = beforePerformerHealth;
    }

    public override void Redo()
    {
        base.Redo();
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        Debug.Log($"Redoing attack on {target.name}.");
        if (target != null)
            target.Health = afterHealth;
        if (drain)
            Performer.CurrentHeatlh = afterPerformerHealth;
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
            if (pierce)
                target.PierceDamage(damage + Performer.GetStatus(Status.Strength));
            else
                target.Damage(damage + Performer.GetStatus(Status.Strength));

            if (drain)
            {
                beforePerformerHealth = Performer.Health;
                Performer.Heal(beforeHealth - target.Health);
                afterPerformerHealth = Performer.Health;
            }

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
        return new List<Vector3Int> { TargetPosition * new Vector3Int(1,1,0) };
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

    public override AbilityBuilder SetAmount(int amount)
    {
        attackAbility.damage = amount;
        return base.SetAmount(amount);
    }

    public override AbilityBuilder SetRange(int range)
    {
        attackAbility.range = range;
        return this;
    }
}