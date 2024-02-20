using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Basic Card")]
[Serializable]
public class Card : ScriptableObject
{
    [Header("Card Details")]
    public string cardName = "Card";
    public string description;
    public bool canUpgrade = true;
    [Space(5)]
    public int cost;
    public int range;
    public CastType castType;

    [Header("Actions List")]
    [SerializeField]
    public AbilityWrapper[] abilities;
    private Guid ID = Guid.NewGuid();

    public Card(Card card)
    {
        this.cardName = card.cardName;
        this.description = card.description;
        this.cost = card.cost;
        this.range = card.range;
        this.castType = card.castType;

        List<AbilityWrapper> cardAbilities = new List<AbilityWrapper>();
        foreach (AbilityWrapper wrapper in card.abilities)
        {
            // Reconstruct Abilities
            cardAbilities.Add(new AbilityWrapper(
                AbilityBuilder.GetBuilder(wrapper.ability)
                                .SetRange(card.range)
                                .SetPerformer(PlayerManager.Instance.Player)
                                .SetEntityMask(new List<string>() { "Enemy"})
                                .Build()));
        }
        abilities = cardAbilities.ToArray();
    }

    public virtual void Play()
    {
        UseEnergyAction useEnegy = new UseEnergyAction() { amount = cost };
        useEnegy.Perform();

        Vector3Int targetPosition = GridManager.Instance.GetGridPositionFromWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        foreach (AbilityWrapper wrapper in abilities)
        {
            Ability ability = wrapper.ability;
            AbilityBuilder builder = AbilityBuilder.GetBuilder(ability);
            GridEntity targetEntity = GridManager.Instance.GetEntityOnPosition(targetPosition);
            ability = builder.SetPerformer(PlayerManager.Instance.Player).SetTargetPosition(targetPosition).SetRange(range).Build();
            ability.Perform();
            ;
        }

        PlayerManager.Instance.discardPile.AddCard(this);
        UndoRedoManager.Instance.AddUndoAction(new PlayCardAction(this));
    }

    private void CastOnEntitiesAtTargetPosition(Vector3Int position, List<Entity> entities)
    {
        foreach (AbilityWrapper wrapper in abilities)
        {
            Ability ability = wrapper.ability;
            switch (ability)
            {
                case SimpleAttackAbility:
                    break;
            }
        }
    }

    public void UpgradeCard()
    {
        foreach (AbilityWrapper wrapper in abilities)
        {
            AbilityBuilder builder = AbilityBuilder.GetBuilder(wrapper.ability);
        }
    }

    public virtual bool CanPlay(Vector3Int position)
    {
        bool result = true;

        if (PlayerManager.Instance.CurrentEnergy - cost < 0)
            return false;

        if (GridManager.Instance.GetWalkingDistance(PlayerManager.Instance.Player.targetGridPosition, position) > range)
            return false;

        foreach (AbilityWrapper wrapper in abilities)
        {
            Vector3Int targetPosition = GridManager.Instance.GetGridPositionFromWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            Ability ability = wrapper.ability;
            AbilityBuilder builder = AbilityBuilder.GetBuilder(ability);
            GridEntity targetEntity = GridManager.Instance.GetEntityOnPosition(targetPosition);
            ability = builder.SetPerformer(PlayerManager.Instance.Player).SetTargetPosition(targetPosition).SetRange(range).Build();

            if (!ability.CanPerform(position))
            {
                return false;
            }
        }
        return result;
    }

    public override bool Equals(object other)
    {
        return this.ID.Equals(((Card)other).ID);
    }
}

