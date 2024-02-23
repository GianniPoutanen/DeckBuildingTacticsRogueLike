
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Status/Give Status Ability")]
public class GiveTargetStatusAbility : Ability
{
    public Status status;
    public int amount;
    private int beforeStatusAmount = 0;
    private int afterStatusAmount = 0;

    public override void Undo()
    {
        base.Undo();
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        if (target != null)
            target.SetStatus(status, beforeStatusAmount);
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Perform()
    {
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        base.Perform();
        if (target != null)
        {
            Debug.Log($"{Performer.name} gives status {status.ToString()} to {target.name} of amount {amount}.");
            beforeStatusAmount = target.GetStatus(status);
            target.GiveStatus(status, amount);
            afterStatusAmount = target.GetStatus(status);
            UIManager.Instance.UpdateUI();
        }
    }
}


// Builder class for GiveStatusAbility
public class GiveTargetStatusBuilder : AbilityBuilder
{
    private GiveTargetStatusAbility giveStatusAbility;

    public GiveTargetStatusBuilder(GiveTargetStatusAbility ability)
    {
        giveStatusAbility = ability;
    }

    public override Ability Build()
    {
        return giveStatusAbility;
    }

    public override AbilityBuilder SetCost(int cost)
    {
        giveStatusAbility.cost = cost;
        return base.SetCost(cost);
    }

    public GiveTargetStatusBuilder SetStatus(Status status)
    {
        giveStatusAbility.status = status;
        return this;
    }
    public override AbilityBuilder SetAmount(int amount)
    {
        giveStatusAbility.amount = amount;
        return base.SetAmount(amount);
    }
    public override AbilityBuilder SetEntityMask(List<string> mask)
    {
        giveStatusAbility.entityMask = mask;
        return base.SetEntityMask(mask);
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        giveStatusAbility.Performer = performer;
        return base.SetPerformer(performer);
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        giveStatusAbility.TargetPosition = position;
        return base.SetTargetPosition(position);
    }
}