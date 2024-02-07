
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attacks/Straight Attack Action")]
public class StraightAttackAbility : Ability
{
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterHealth = new Dictionary<GridEntity, int>();
    public int damage;
    public int range;
    public Vector3Int targetPosition;

    public override void Undo()
    {
        foreach (var target in targets)
        {
            Debug.Log($"Undoing attack on {target.name}.");
            target.Health = beforeHealth[target];
        }
    }

    public override void Redo()
    {
        foreach (var target in targets)
        {
            Debug.Log($"Redoing attack on {target.name}.");
            target.Health = afterHealth[target];
        }
    }
    public override void Perform()
    {
        base.Perform();
        foreach (var target in targets)
        {
            Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
            beforeHealth.Add(target, target.Health);
            target.Damage(damage);
            afterHealth.Add(target, target.Health);
        }
    }

    public override bool CanPerform(Vector3Int position)
    {
        return targets.Count > 0;
    }


}

public class StraightAttackBuilder : AbilityBuilder
{
    StraightAttackAbility moveSelfAbility = new StraightAttackAbility();

    public override Ability Build()
    {
        return moveSelfAbility;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        moveSelfAbility.Performer = performer;
        return SetPerformer(performer);
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        moveSelfAbility.targetPosition = position;
        return SetTargetPosition(position);
    }

    public override AbilityBuilder SetDamage(int amount)
    {
        moveSelfAbility.damage = amount;
        return base.SetDamage(amount);
    }
}