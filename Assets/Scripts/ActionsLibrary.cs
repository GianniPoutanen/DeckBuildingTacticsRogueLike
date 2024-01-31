
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Ability : ScriptableObject, IUndoRedoAction
{
    [HideInInspector]
    public GridEntity _performer = null;
    [HideInInspector]
    public GridEntity Performer { get { return _performer; } set { _performer = value; } }


    public virtual void Redo()
    {
        throw new NotImplementedException();
    }

    public virtual void Undo()
    {
        throw new NotImplementedException();
    }
    public virtual void Perform(Vector3Int position)
    {
        throw new NotImplementedException();
    }

    public virtual bool CanPlay(Vector3Int position)
    {
        return true;
    }
}

[System.Serializable]
public class AbilityWrapper : IUndoRedoAction
{
    public Ability ability;

    public AbilityWrapper(Ability action)
    {
        this.ability = action;
    }

    public GridEntity Performer { get { return ability.Performer; } set { ability.Performer = value; } }

    public void Perform(Vector3Int position)
    {
        ability.Perform(position);
    }

    public void Redo()
    {
        ability.Redo();
    }

    public void Undo()
    {
        ability.Undo();
    }
}

public class CompositeAction : Ability
{
    public List<Ability> actions = new List<Ability>();

    public override void Undo()
    {
        for (int i = 0; i <= actions.Count - 1; i++)
        {
            actions[i].Undo();
        }
    }

    public override void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Redo();
        }
    }

    public override void Perform(Vector3Int position)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Perform(position);
        }
    }

    public override bool CanPlay(Vector3Int pos)
    {
        bool result = true;
        foreach (Ability action in actions)
        {
            if (!action.CanPlay(pos))
            {
                result = false;
                break;
            }
        }
        return result;
    }
}


/// <summary>
/// GRID ENTITY ACTIONS
/// </summary>
public class MoveGridEntityAction : Ability
{
    public GridEntity target;
    public Vector3Int oldPosition;
    public Vector3Int newPosition;


    public override void Undo()
    {
        target.transform.position = oldPosition;
        target.targetGridPosition = oldPosition;
    }

    public override void Redo()
    {
        target.transform.position = newPosition;
        target.targetGridPosition = newPosition;
    }

    public override void Perform(Vector3Int position)
    {
        Debug.Log("Moving " + target + " from " + oldPosition + " to " + newPosition);
        target.targetGridPosition = newPosition;
    }

    public override bool CanPlay(Vector3Int position)
    {
        return target.CanMoveTo(position);
    }
}

public class UseEnergyAction : Ability
{
    private int newEnergyAmount;
    private int oldEnergyAmount;
    public int amount = 0;

    public override void Perform(Vector3Int position)
    {
        oldEnergyAmount = PlayerManager.Instance.CurrentEnergy;
        newEnergyAmount = PlayerManager.Instance.CurrentEnergy - amount;
        PlayerManager.Instance.CurrentEnergy = newEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public override void Redo()
    {
        PlayerManager.Instance.CurrentEnergy = newEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public override void Undo()
    {
        PlayerManager.Instance.CurrentEnergy = oldEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public override bool CanPlay(Vector3Int position)
    {
        return PlayerManager.Instance.CurrentEnergy - amount >= 0;
    }
}

#region Attack Abilities
[CreateAssetMenu(menuName = "Actions/Attack Action")]
public class AttackAbility : Ability
{
    [HideInInspector]
    public GridEntity attacker;
    [HideInInspector]
    public GridEntity target;
    private int beforeHealth;
    private int afterHealth;
    private int beforeArmour;
    private int afterArmour;
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
    public override void Perform(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);

        Debug.Log($"{attacker.name} attacks {target.name} for {damage} damage.");
        beforeArmour = target.Armour;
        beforeHealth = target.Health;
        if (piercing)
            target.PierceDamage(damage);
        else
            target.Damage(damage);
        afterArmour = target.Armour;
        afterHealth = target.Health;
    }

    public override bool CanPlay(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);
        return target != null;
    }
}

[CreateAssetMenu(menuName = "Actions/Cleave Attack Action")]
public class CleaveAttackAction : Ability
{
    [HideInInspector]
    public GridEntity attacker;
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterHealth = new Dictionary<GridEntity, int>();
    public int damage;
    public int distance = 2;

    public override void Undo()
    {
        foreach (var target in targets)
        {
            Debug.Log($"Undoing attack on {target.name}.");
            target.Health = beforeHealth[target];
        }
    }

    public override void Redo()
    {
        foreach (var target in targets)
        {
            Debug.Log($"Redoing attack on {target.name}.");
            target.Health = afterHealth[target];
        }
    }
    public override void Perform(Vector3Int position)
    {


        foreach (var target in targets)
        {
            Debug.Log($"{attacker.name} attacks {target.name} for {damage} damage.");
            beforeHealth.Add(target, target.Health);
            target.Damage(damage);
            afterHealth.Add(target, target.Health);
        }
    }

    public override bool CanPlay(Vector3Int position)
    {
        if (attacker != null)
        {
            float angle = 0;
            // TODO add grid manager logic for cleaving in cone
            GridManager.Instance.GetPositionsInCone(attacker.targetGridPosition, distance, angle);
            targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInCone(attacker.targetGridPosition, distance, angle));
            return targets.Count > 0;

        }
        return false;
    }
}

[CreateAssetMenu(menuName = "Actions/Straight Attack Action")]
public class StraightAttackAction : Ability
{
    [HideInInspector]
    public GridEntity attacker;
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterHealth = new Dictionary<GridEntity, int>();
    public int damage;


    public override void Undo()
    {
        foreach (var target in targets)
        {
            Debug.Log($"Undoing attack on {target.name}.");
            target.Health = beforeHealth[target];
        }
    }

    public override void Redo()
    {
        foreach (var target in targets)
        {
            Debug.Log($"Redoing attack on {target.name}.");
            target.Health = afterHealth[target];
        }
    }
    public override void Perform(Vector3Int position)
    {
        foreach (var target in targets)
        {
            Debug.Log($"{attacker.name} attacks {target.name} for {damage} damage.");
            beforeHealth.Add(target, target.Health);
            target.Damage(damage);
            afterHealth.Add(target, target.Health);
        }
    }

    public override bool CanPlay(Vector3Int position)
    {
        return targets.Count > 0;
    }
}


#endregion Attack Abilities

#region Healing / Shielding Abilities
[CreateAssetMenu(menuName = "Heal Or Shield/Heal Action")]
public class HealAction : Ability
{
    [HideInInspector]
    public GridEntity healer;
    [HideInInspector]
    public GridEntity target;
    private int beforeHealth;
    private int afterHealth;
    public int healingAmount;


    public override void Undo()
    {
        Debug.Log($"Undoing healing on {target.name}.");
        target.Health = beforeHealth;
    }

    public override void Redo()
    {
        Debug.Log($"Redoing healing on {target.name}.");
        target.Health = afterHealth;
    }

    public override void Perform(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);

        Debug.Log($"{healer.name} heals {target.name} for {healingAmount} health.");
        beforeHealth = target.Health;
        target.Heal(healingAmount);
        afterHealth = target.Health;
    }
}

[CreateAssetMenu(menuName = "Heal Or Shield/Shield Action")]
public class ShieldAction : Ability
{
    [HideInInspector]
    public GridEntity shielder;
    [HideInInspector]
    public GridEntity target;
    private int beforeArmour;
    private int afterArmour;
    public int armourAmount;


    public override void Undo()
    {
        Debug.Log($"Undoing armour on {target.name}.");
        target.Armour = beforeArmour;
    }

    public override void Redo()
    {
        Debug.Log($"Redoing armour on {target.name}.");
        target.Armour = afterArmour;
    }

    public override void Perform(Vector3Int position)
    {
        target = GridManager.Instance.GetEntityOnPosition(position);

        Debug.Log($"{shielder.Armour} shields {target.name} for {armourAmount} health.");
        beforeArmour = target.Health;
        target.Shield(armourAmount);
        afterArmour = target.Armour;
    }
}

[CreateAssetMenu(menuName = "Heal Or Shield/Heal Group Action")]
public class HealGroupAction : Ability
{
    [HideInInspector]
    public GridEntity healer;
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealthDict;
    private Dictionary<GridEntity, int> afterHealthDict;
    public int healingAmount;

    public override void Undo()
    {
        Debug.Log($"Undoing healing on all targets.");
        foreach (GridEntity target in targets)
        {
            target.Health = beforeHealthDict[target];
        }
    }

    public override void Redo()
    {
        Debug.Log($"Redoing healing on all targets.");
        foreach (GridEntity target in targets)
        {
            target.Health = afterHealthDict[target];
        }
    }

    public override void Perform(Vector3Int position)
    {
        beforeHealthDict = new Dictionary<GridEntity, int>();
        afterHealthDict = new Dictionary<GridEntity, int>();

        foreach (GridEntity target in targets)
        {
            Debug.Log($"{healer.name} heals {target.name} for {healingAmount} health.");
            beforeHealthDict.Add(target, target.Health);
            target.Heal(healingAmount);
            afterHealthDict.Add(target, target.Health);
        }
    }
}

[CreateAssetMenu(menuName = "Heal Or Shield/Shield Group Action")]
public class ShieldGroupAction : Ability
{
    [HideInInspector]
    public GridEntity shielder;
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeArmourDict;
    private Dictionary<GridEntity, int> afterArmourDict;
    public int armourAmount;

    public override void Undo()
    {
        Debug.Log($"Undoing armour on all targets.");
        foreach (GridEntity target in targets)
        {
            target.Armour = beforeArmourDict[target];
        }
    }

    public override void Redo()
    {
        Debug.Log($"Redoing armour on all targets.");
        foreach (GridEntity target in targets)
        {
            target.Armour = afterArmourDict[target];
        }
    }

    public override void Perform(Vector3Int position)
    {
        beforeArmourDict = new Dictionary<GridEntity, int>();
        afterArmourDict = new Dictionary<GridEntity, int>();

        foreach (GridEntity target in targets)
        {
            Debug.Log($"{shielder.Armour} shields {target.name} for {armourAmount} health.");
            beforeArmourDict.Add(target, target.Armour);
            target.Shield(armourAmount);
            afterArmourDict.Add(target, target.Armour);
        }
    }
}

#endregion Healing / Shielding Abilities


public class CardPlayedAction : Ability, IUndoRedoAction
{
    private Card card;

    public CardPlayedAction(Card card)
    {
        this.card = card;
    }

    public override void Perform(Vector3Int position)
    {
        foreach (var action in card.abilities)
        {
            action.Perform(position);
        }
    }

    public override void Undo()
    {
        for (int i = card.abilities.Length - 1; i >= 0; i--)
        {
            card.abilities[i].Undo();
        }
    }

    public override void Redo()
    {
        foreach (var action in card.abilities)
        {
            action.Redo();
        }
    }
}
