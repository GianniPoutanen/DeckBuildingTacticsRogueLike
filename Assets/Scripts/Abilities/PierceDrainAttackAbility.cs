using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceDrainAttackAbility : Ability
{
    [HideInInspector]
    public GridEntity target;
    private int beforeHealth;
    private int afterHealth;
    public bool piercing = false;
    public int damage;
    public Vector3Int targetPosition;

    public override void Undo()
    {
        Debug.Log($"Undoing attack on {target.name}.");
        target.Health = beforeHealth;
    }

    public override void Redo()
    {
        Debug.Log($"Redoing attack on {target.name}.");
        target.Health = afterHealth;
    }
    public override void Perform()
    {
        base.Perform();
        target = GridManager.Instance.GetEntityOnPosition(targetPosition);

        Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
        beforeHealth = target.Health;
        target.PierceDamage(damage);
        _performer.Heal(damage);
        afterHealth = target.Health;
    }

    public override bool CanPerform(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);
        return target != null;
    }
}

public class PierceDrainAttackBuilder : AbilityBuilder
{
    PierceDrainAttackAbility attackAbility = new PierceDrainAttackAbility();

    public override Ability Build()
    {
        return attackAbility;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        attackAbility.Performer = performer;
        return this;
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        attackAbility.targetPosition = position;
        return this;
    }

}
