
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stat Changes/Heal Self Ability")]
public class HealSelfAbility : Ability
{
    public int amount;
    private int beforeHealthAmount = 0;
    private int afterHealthAmount = 0;

    public override void Undo()
    {
        base.Undo();
        Performer.Health = beforeHealthAmount;
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Perform()
    {
        base.Perform();
        Debug.Log($"{Performer.name} healing {amount}.");
        beforeHealthAmount = Performer.Health;
        Performer.Heal(amount);
        afterHealthAmount = Performer.Health;
        UIManager.Instance.UpdateUI();
    }
}

// Builder class for HealSelfAbility
public class HealSelfBuilder : AbilityBuilder
{
    private HealSelfAbility healSelfAbility;

    public HealSelfBuilder(HealSelfAbility ability)
    {
        healSelfAbility = ability;
    }

    public override Ability Build()
    {
        return healSelfAbility;
    }

    public override AbilityBuilder SetCost(int cost)
    {
        healSelfAbility.cost = cost;
        return base.SetCost(cost);
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        healSelfAbility.Performer = performer;
        return base.SetPerformer(performer);
    }

    public HealSelfBuilder SetAmount(int amount)
    {
        healSelfAbility.amount = amount;
        return this;
    }
}