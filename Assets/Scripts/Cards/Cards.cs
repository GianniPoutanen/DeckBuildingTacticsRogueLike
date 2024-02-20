using System;
using System.Collections.Generic;
using System.Linq;
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
    public int Cost { get { return abilities.Sum(x => x.ability.cost );  } }
    [Space(5)]
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

    #region Cast Types
    private void SimpleCast(Vector3Int targetPosition)
    {
        // Implement logic for simple cast at the target position
        Debug.Log($"Simple cast at position: {targetPosition}");

        foreach (AbilityWrapper wrapper in abilities)
        {
            Ability ability = wrapper.ability;
            AbilityBuilder builder = AbilityBuilder.GetBuilder(ability);
            GridEntity targetEntity = GridManager.Instance.GetEntityOnPosition(targetPosition);
            ability = builder.SetPerformer(PlayerManager.Instance.Player).SetTargetPosition(targetPosition).SetRange(range).Build();
        }
    }

    // TODO
    /*
    private void WithinDistanceCast(Vector3Int targetPosition)
    {
        // Implement logic for casting within a given distance of the target position
        // For example, use the GetPositionsInDistance function provided earlier
        int distance = 2;
        List<Vector3Int> positionsWithinDistance = GridManager.Instance.GetValidPositionsInDistance(targetPosition, distance);
        List<GridEntity> entities = GridManager.Instance.GetEntitiesOnPositions(positionsWithinDistance);

        GridManager.Instance.UpdateCastPositionsTilemap(positionsWithinDistance);

        Debug.Log($"Casting within distance at positions: {string.Join(", ", positionsWithinDistance)}");
    }

    private void ConeCast(Vector3Int targetPosition)
    {
        // Implement logic for casting in a cone from the target position
        // For example, determine positions within a cone angle from the target position
        float coneAngle = 45f;
        List<Vector3Int> positionsInCone = GridManager.Instance.GetPositionsInCone(targetPosition, range, coneAngle);
        List<GridEntity> entities = GridManager.Instance.GetEntitiesOnPositions(positionsInCone);

        GridManager.Instance.UpdateCastPositionsTilemap(positionsInCone);

        Debug.Log($"Cone cast at positions: {string.Join(", ", positionsInCone)}");
    }

    private void AreaCast(Vector3Int targetPosition)
    {
        // Implement logic for casting in an area surrounding the target position
        // For example, determine positions within a square area around the target position
        int areaSize = 3;
        List<Vector3Int> positionsInArea = GridManager.Instance.GetPositionsInArea(targetPosition, areaSize);
        List<GridEntity> entities = GridManager.Instance.GetEntitiesOnPositions(positionsInArea);

        GridManager.Instance.UpdateCastPositionsTilemap(positionsInArea);

        Debug.Log($"Area cast at positions: {string.Join(", ", positionsInArea)}");
    }
    */
    #endregion Cast Types
    public virtual bool CanPlay(Vector3Int position)
    {
        bool result = true;

        if (PlayerManager.Instance.CurrentEnergy - Cost < 0)
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

