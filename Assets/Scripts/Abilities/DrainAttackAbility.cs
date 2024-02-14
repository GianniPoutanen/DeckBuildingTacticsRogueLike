using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attacks/Drain Attack Action")]
public class DrainAttackAbility : Ability
{
    [HideInInspector]
    public GridEntity target;
    private int beforeArmour;
    private int beforeHealth;
    private int afterArmour;
    private int afterHealth;
    public int damage;

    public override void Undo()
    {
        Debug.Log($"Undoing attack on {target.name}.");
        target.Health = beforeHealth;
        target.Armour = beforeArmour;
    }

    public override void Redo()
    {
        Debug.Log($"Redoing attack on {target.name}.");
        target.Health = afterHealth;
        target.Armour = afterArmour;
    }
    public override void Perform()
    {
        base.Perform();
        target = GridManager.Instance.GetEntityOnPosition(TargetPosition);

        Debug.Log($"{Performer.name} attacks {target.name} for {damage} damage.");
        beforeHealth = target.Health;
        beforeArmour = target.Armour;
        int drainAmount = damage - target.Armour;
        target.Damage(damage);
        Performer.Heal(drainAmount > 0 ? drainAmount : 0);
        afterHealth = target.Health;
        afterArmour = target.Armour;
    }

    public override bool CanPerform(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);
        return target != null;
    }
}


public class DrainAttackBuilder : AbilityBuilder
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