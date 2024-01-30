using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;

[Serializable]
public class Card
{
    [Header("Card Details")]
    public string cardName = "Card";
    public string description;
    [Space(5)]
    public int cost;
    public int range;
    public CastType castType;

    [Header("Actions List")]
    [SerializeField]
    public AbilityWrapper[] abilities;


    public virtual void Play()
    {
        UndoRedoManager.Instance.AddUndoAction(new UseEnergyAction() { amount = cost });
        PlayerManager.Instance.CurrentEnergy -= cost;

    }

    public void Cast(Vector3Int targetPosition)
    {
        switch (castType)
        {
            case CastType.Simple:
                SimpleCast(targetPosition);
                break;

            case CastType.WithinDistance:
                WithinDistanceCast(targetPosition);
                break;

            case CastType.Cone:
                ConeCast(targetPosition);
                break;

            case CastType.Area:
                AreaCast(targetPosition);
                break;

            // Add more cases for other cast types...

            default:
                Debug.LogError("Invalid cast type");
                break;
        }
    }

    private void CastOnEntitiesAtTargetPosition(Vector3Int position, List<Entity> entities)
    {
        foreach (AbilityWrapper wrapper in abilities)
        {
            Ability ability = wrapper.ability;
            switch (ability)
            {
                case AttackAbility:
                    break;
            }
        }

    }


    #region Cast Types
    private void SimpleCast(Vector3Int targetPosition)
    {
        // Implement logic for simple cast at the target position
        Debug.Log($"Simple cast at position: {targetPosition}");
    }

    private void WithinDistanceCast(Vector3Int targetPosition)
    {
        // Implement logic for casting within a given distance of the target position
        // For example, use the GetPositionsInDistance function provided earlier
        int distance = 2;
        List<Vector3Int> positionsWithinDistance = GridManager.Instance.GetValidPositionsInDistance(targetPosition, distance);
        List<GridEntity> entities = GridManager.Instance.GetEntitiesOnPositions(positionsWithinDistance);

        GridManager.Instance.UpdateSelectedTilemap(positionsWithinDistance);

        Debug.Log($"Casting within distance at positions: {string.Join(", ", positionsWithinDistance)}");
    }

    private void ConeCast(Vector3Int targetPosition)
    {
        // Implement logic for casting in a cone from the target position
        // For example, determine positions within a cone angle from the target position
        float coneAngle = 45f;
        List<Vector3Int> positionsInCone = GridManager.Instance.GetPositionsInCone(targetPosition, range, coneAngle);
        List<GridEntity> entities = GridManager.Instance.GetEntitiesOnPositions(positionsInCone);

        GridManager.Instance.UpdateSelectedTilemap(positionsInCone);

        Debug.Log($"Cone cast at positions: {string.Join(", ", positionsInCone)}");
    }

    private void AreaCast(Vector3Int targetPosition)
    {
        // Implement logic for casting in an area surrounding the target position
        // For example, determine positions within a square area around the target position
        int areaSize = 3;
        List<Vector3Int> positionsInArea = GridManager.Instance.GetPositionsInArea(targetPosition, areaSize);
        List<GridEntity> entities = GridManager.Instance.GetEntitiesOnPositions(positionsInArea);

        GridManager.Instance.UpdateSelectedTilemap(positionsInArea);

        Debug.Log($"Area cast at positions: {string.Join(", ", positionsInArea)}");
    }

    #endregion Cast Types


    public virtual bool CanPlay(Vector3Int position)
    {
        // This method can be overridden in the subclasses
        if (PlayerManager.Instance.CurrentEnergy >= cost)
            return true;
        return false;
    }
}

public class SlashCard : Card
{
    public int damage = 2;

    public override void Play()
    {
        base.Play();
    }
}


public class AttackCard : Card
{
    public int damage;

    public override void Play()
    {
        base.Play();
    }
}

public class DefenseCard : Card
{
    public int armor;

    public override void Play()
    {
        base.Play();
    }
}

public class SkillCard : Card
{
    public string effect;

    public override void Play()
    {
        base.Play();
    }
}
