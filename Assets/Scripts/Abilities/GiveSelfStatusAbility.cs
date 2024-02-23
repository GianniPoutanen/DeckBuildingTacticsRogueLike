
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Status/Give Self Status Ability")]
public class GiveSelfStatusAbility : Ability
{
    public Status status;
    public int amount;
    private int beforeStatusAmount = 0;
    private int afterStatusAmount = 0;

    public override void Undo()
    {
        base.Undo();
        Performer.SetStatus(status, beforeStatusAmount);
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Perform()
    {
        base.Perform();
        Debug.Log($"{Performer.name} gains status {status.ToString()} of amount {amount}.");
        beforeStatusAmount = Performer.GetStatus(status);
        Performer.GiveStatus(status, amount);
        afterStatusAmount = Performer.GetStatus(status);
        UIManager.Instance.UpdateUI();
    }
}

// Builder class for GiveSelfStatusAbility
public class GiveSelfStatusBuilder : AbilityBuilder
{
    private GiveSelfStatusAbility giveSelfStatusAbility;

    public GiveSelfStatusBuilder(GiveSelfStatusAbility ability)
    {
        giveSelfStatusAbility = ability;
    }

    public override Ability Build()
    {
        return giveSelfStatusAbility;
    }

    public override AbilityBuilder SetCost(int cost)
    {
        giveSelfStatusAbility.cost = cost;
        return base.SetCost(cost);
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        giveSelfStatusAbility.Performer = performer;
        return base.SetPerformer(performer);
    }

    public override AbilityBuilder SetAmount(int amount)
    {
        giveSelfStatusAbility.amount = amount;
        return base.SetAmount(amount);
    }
}