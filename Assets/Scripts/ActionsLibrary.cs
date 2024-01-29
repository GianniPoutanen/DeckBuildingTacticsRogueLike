
using System.Collections.Generic;
using UnityEngine;

public class CompositeAction : IUndoRedoAction
{
    private List<IUndoRedoAction> actions = new List<IUndoRedoAction>();
    private Entity entity;
    public CompositeAction(List<IUndoRedoAction> actions, Entity entity)
    {
        this.actions = actions;
        this.entity = entity;
    }

    public void Undo()
    {
        for (int i = 0; i <= actions.Count - 1; i++)
        {
            actions[i].Undo();
        }
    }

    public void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Redo();
        }
    }

    public Entity GetEntity()
    {
        return entity;
    }
}


/// <summary>
/// GRID ENTITY ACTIONS
/// </summary>
public class MoveGridEntityAction : IUndoRedoAction
{
    private GridEntity gridEntity;
    private Vector3Int oldPosition;
    private Vector3Int newPosition;
    private Entity entity;

    public MoveGridEntityAction(GridEntity entity, Vector3Int oldPosition, Vector3Int newPosition)
    {
        this.gridEntity = entity;
        this.oldPosition = oldPosition;
        this.newPosition = newPosition;
    }

    public void Undo()
    {
        gridEntity.transform.position = oldPosition;
        gridEntity.targetGridPosition = oldPosition;
    }

    public void Redo()
    {
        gridEntity.transform.position = newPosition;
        gridEntity.targetGridPosition = newPosition;
    }
    public Entity GetEntity()
    {
        return gridEntity;
    }
}

public class UseEnergyAction : IUndoRedoAction
{
    private int newEnergyAmount = 0;
    private int oldEnergyAmount = 0;
    public UseEnergyAction(int newAmount, int oldAmount)
    {
        newEnergyAmount = newAmount;
        oldEnergyAmount = oldAmount;
    }

    public Entity GetEntity()
    {
        return PlayerManager.Instance.Player;
    }

    public void Redo()
    {
        PlayerManager.Instance.CurrentEnergy = newEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public void Undo()
    {
        PlayerManager.Instance.CurrentEnergy = oldEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }
}
public class AttackAction : IAttackAction
{
    private Entity attacker;
    private Entity target;
    private int beforeHealth;
    private int afterHealth;
    private int damage;

    public AttackAction(Entity attacker, Entity target, int damage)
    {
        this.attacker = attacker;
        this.target = target;
        this.beforeHealth = target.Health;
        this.afterHealth = Mathf.Max(0, target.Health - damage);
        this.damage = damage;
    }

    public void ExecuteAttack()
    {
        // Logic to perform the attack
        // Example: Deal damage to the target entity
        Debug.Log($"{attacker.name} attacks {target.name} for {damage} damage.");
        target.Health = afterHealth;
        // Additional logic can be added based on your game's combat system
    }

    public void Undo()
    {
        // Logic to undo the attack
        // Example: Revert the damage done to the target entity
        Debug.Log($"Undoing attack on {target.name}.");
        target.Health = beforeHealth;
        // Additional undo logic based on your game's mechanics
    }

    public void Redo()
    {
        // Logic to redo the attack
        // Example: Reapply damage to the target entity
        Debug.Log($"Redoing attack on {target.name}.");
        target.Health = afterHealth;
        // Additional redo logic based on your game's mechanics
    }
    public Entity GetEntity()
    {
        return attacker;
    }
}

public class HealAction : IHealAction
{
    private Entity healer;
    private Entity target;
    private int beforeHealth;
    private int afterHealth;
    private int healingAmount;

    public HealAction(Entity healer, Entity target, int healingAmount)
    {
        this.healer = healer;
        this.target = target;
        this.beforeHealth = target.Health;
        this.afterHealth = target.Health + healingAmount;
        this.healingAmount = healingAmount;
    }

    public void ExecuteHeal()
    {
        // Logic to perform the healing
        // Example: Heal the target entity
        Debug.Log($"{healer.name} heals {target.name} for {healingAmount} health.");
        target.Health += healingAmount;
        afterHealth = target.Health;
        // Additional healing logic based on your game's mechanics
    }

    public void Undo()
    {
        // Logic to undo the healing
        // Example: Revert the healing done to the target entity
        Debug.Log($"Undoing healing on {target.name}.");
        target.Health = beforeHealth;
        // Additional undo logic based on your game's mechanics
    }

    public void Redo()
    {
        // Logic to redo the healing
        // Example: Reapply healing to the target entity
        Debug.Log($"Redoing healing on {target.name}.");
        target.Health = afterHealth;
        // Additional redo logic based on your game's mechanics
    }
    public Entity GetEntity()
    {
        return healer;
    }
}

public class UseAbilityAction : IUseAbilityAction
{
    private Entity user;
    private Vector3 abilityTargetPosition;

    public UseAbilityAction(Entity user, Vector3 abilityTargetPosition)
    {
        this.user = user;
        this.abilityTargetPosition = abilityTargetPosition;
    }

    public void ExecuteAbility()
    {
        // Logic to perform the ability
        // Example: Use a special ability at the target position
        Debug.Log($"{user.name} uses an ability at position {abilityTargetPosition}.");
        // Additional ability logic based on your game's mechanics
    }

    public void Undo()
    {
        // Logic to undo the ability
        // Example: Revert the effects of the ability
        Debug.Log($"Undoing ability used by {user.name}.");
        // Additional undo logic based on your game's mechanics
    }

    public void Redo()
    {
        // Logic to redo the ability
        // Example: Reapply the effects of the ability
        Debug.Log($"Redoing ability used by {user.name}.");
        // Additional redo logic based on your game's mechanics
    }

    public Entity GetEntity()
    {
        return user;
    }
}

public class PlayerCardAction : IUndoRedoAction
{
    private Card card;

    public PlayerCardAction(Card card)
    {

    }

    public Entity GetEntity()
    {
        throw new System.NotImplementedException();
    }

    public void Redo()
    {
        throw new System.NotImplementedException();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }
}
