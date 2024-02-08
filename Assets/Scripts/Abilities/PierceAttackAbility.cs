using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attacks/Pierce Attack Action")]
public class PierceAttackAbility : Ability
{
    [HideInInspector]
    public GridEntity target;
    private int beforeHealth;
    private int afterHealth;
    public bool piercing = false;
    public int damage;

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
        target = GridManager.Instance.GetEntityOnPosition(TargetPosition);

        Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
        beforeHealth = target.Health;
        target.PierceDamage(damage);
        afterHealth = target.Health;
    }

    public override bool CanPerform(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);
        return target != null;
    }
}


public class PierceAttackBuilder : AbilityBuilder
{
    PierceAttackAbility attackAbility = new PierceAttackAbility();

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
        attackAbility.TargetPosition = position;
        return this;
    }
}