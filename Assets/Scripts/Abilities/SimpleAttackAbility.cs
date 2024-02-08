
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
    public Vector3Int targetPosition;
    public int range;

    public override void Undo()
    {
        var target = GridManager.Instance.GetEntityOnPosition(targetPosition, entityMask);
        Debug.Log($"Undoing attack on {target.name}.");
        if (target != null)
            target.Health = beforeHealth;
    }

    public override void Redo()
    {
        var target = GridManager.Instance.GetEntityOnPosition(targetPosition, entityMask);
        Debug.Log($"Redoing attack on {target.name}.");
        if (target != null)
            target.Health = afterHealth;
    }
    public override void Perform()
    {
        var target = GridManager.Instance.GetEntityOnPosition(targetPosition, entityMask);
        base.Perform();
        if (target != null)
        {
            Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
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
        var target = GridManager.Instance.GetEntityOnPosition(targetPosition, entityMask);
        return target != null && target != _performer;
    }

    public override List<Vector3Int> GetAbilityPositions()
    {
        return new List<Vector3Int> { targetPosition };
    }

    public override List<Vector3Int> GetPossiblePositions(Vector3Int originPosition)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        foreach (Vector3Int pos in GridManager.Instance.GetGridPositionsWithinDistance(originPosition, range))
            if (GridManager.Instance.GetWalkingDistance(originPosition, pos) <= range)
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

    public override AbilityBuilder SetDamage(int damage)
    {
        attackAbility.damage = damage;
        return this;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        attackAbility.Performer = performer;
        return this;
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        attackAbility.targetPosition = position * new Vector3Int(1, 1, 0);
        return this;
    }

}