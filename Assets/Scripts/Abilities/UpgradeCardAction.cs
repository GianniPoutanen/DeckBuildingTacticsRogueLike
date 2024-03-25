using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Upgrade")]
public class UpgradeCardAction : Ability
{
    public Card card;
    private Card prevCard;

    public UpgradeCardAction(Card card)
    {
        this.card = card;
        this.prevCard = card;
    }


    public override void Perform()
    {
        base.Perform();
        card = UpgradeCard(card);
    }

    public override void Undo()
    {
        card = prevCard;
    }

    public override void Redo()
    {
        card = UpgradeCard(prevCard);
    }

    private Card UpgradeCard(Card card)
    {
        switch (card.name)
        {
            case "Slice":
                break;

            default:
                Debug.Log($"{card.name} has no upgrade");
                throw new System.NotImplementedException();
        }
        return card;
    }

    private void UpgradeCardAbilities(Card card)
    {
        foreach (AbilityWrapper abilityWrapper in card.abilities)
        {
            switch (abilityWrapper.ability)
            {
                case SimpleAttackAbility:
                    (abilityWrapper.ability as SimpleAttackAbility).damage += 2;
                    break;
                default:
                    Debug.Log($"{card.name} has no upgrade");
                    throw new System.NotImplementedException();
            }
        }
    }

}