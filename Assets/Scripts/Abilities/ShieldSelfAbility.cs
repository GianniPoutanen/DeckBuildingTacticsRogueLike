
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stat Changes/Shield Self Ability")]
public class ShieldSelfAbility : Ability
{
    public int amount;
    private int beforeArmourAmount = 0;
    private int afterArmourAmount = 0;

    public override void Undo()
    {
        base.Undo();
        Performer.Armour = beforeArmourAmount;
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Perform()
    {
        base.Perform();
        Debug.Log($"{Performer.name} healing {amount}.");
        beforeArmourAmount = Performer.Armour;
        Performer.Shield(amount);
        afterArmourAmount = Performer.Armour;
        UIManager.Instance.UpdateUI();
    }
}
public class ShieldSelfBuilder : AbilityBuilder
{
    private ShieldSelfAbility shieldSelfAbility;

    public ShieldSelfBuilder(ShieldSelfAbility ability)
    {
        shieldSelfAbility = ability;
    }

    public override Ability Build()
    {
        return shieldSelfAbility;
    }

    public override AbilityBuilder SetCost(int cost)
    {
        shieldSelfAbility.cost = cost;
        return base.SetCost(cost);
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        shieldSelfAbility.Performer = performer;
        return base.SetPerformer(performer);
    }

    public override AbilityBuilder SetAmount(int amount)
    {
        shieldSelfAbility.amount = amount;
        return base.SetAmount(amount);
    }
}
