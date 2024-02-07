
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

/*
#region Attack Abilities

#region Cleace Attack
[CreateAssetMenu(menuName = "Abilities/Attacks/Cleave Attack Action")]
public class CleaveAttackAbility : Ability
{
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterHealth = new Dictionary<GridEntity, int>();
    public int damage;
    public int distance = 2;
    public Vector3Int targetPosition;

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
    public override void Perform()
    {
        base.Perform();
        foreach (var target in targets)
        {
            Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
            beforeHealth.Add(target, target.Health);
            target.Damage(damage);
            afterHealth.Add(target, target.Health);
        }
    }

    public override bool CanPlay(Vector3Int position)
    {
        if (_performer != null)
        {
            float angle = 0;
            // TODO add grid manager logic for cleaving in cone
            GridManager.Instance.GetPositionsInCone(_performer.targetGridPosition, distance, angle);
            targets = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInCone(_performer.targetGridPosition, distance, angle));
            return targets.Count > 0;

        }
        return false;
    }
}

public class CleaveAttackBuilder : IAbilityBuilder
{
    CleaveAttackAbility cleaveAttackAbility = new CleaveAttackAbility();

    public Ability Build()
    {
        return cleaveAttackAbility;
    }

    public IAbilityBuilder SetPerformer(GridEntity performer)
    {
        cleaveAttackAbility.Performer = performer;
        return this;
    }

    public IAbilityBuilder SetTargetPosition(Vector3Int position)
    {
        cleaveAttackAbility.targetPosition = position;
        return this;
    }

    public IAbilityBuilder SetRange(int range)
    {
        return this;
    }

    public IAbilityBuilder SetTargetEntity(GridEntity entity)
    {
        return this;
    }

    public IAbilityBuilder SetCard(Card card)
    {
        throw new NotImplementedException();
    }
}

#endregion Cleace Attack

#region Straight Attack
[CreateAssetMenu(menuName = "Abilities/Attacks/Straight Attack Action")]
public class StraightAttackAbility : Ability
{
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealth = new Dictionary<GridEntity, int>();
    private Dictionary<GridEntity, int> afterHealth = new Dictionary<GridEntity, int>();
    public int damage;
    public Vector3Int targetPosition;

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
    public override void Perform()
    {
        base.Perform();
        foreach (var target in targets)
        {
            Debug.Log($"{_performer.name} attacks {target.name} for {damage} damage.");
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

public class StraightAttackBuilder : IAbilityBuilder
{
    StraightAttackAbility moveSelfAbility = new StraightAttackAbility();

    public Ability Build()
    {
        return moveSelfAbility;
    }

    public IAbilityBuilder SetPerformer(GridEntity performer)
    {
        moveSelfAbility.Performer = performer;
        return this;
    }

    public IAbilityBuilder SetTargetPosition(Vector3Int position)
    {
        moveSelfAbility.targetPosition = position;
        return this;
    }

    public IAbilityBuilder SetRange(int range)
    {
        return this;
    }

    public IAbilityBuilder SetTargetEntity(GridEntity entity)
    {
        return this;
    }

    public IAbilityBuilder SetCard(Card card)
    {
        throw new NotImplementedException();
    }
}

#endregion Straight Attack

#endregion Attack Abilities

#region Healing / Shielding Abilities

#region Heal Target
[CreateAssetMenu(menuName = "Abilities/Heal Or Shield/Heal Action")]
public class HealTargetAbility : Ability
{
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

    public override void Perform()
    {
        base.Perform();
        Debug.Log($"{_performer.name} heals {target.name} for {healingAmount} health.");
        beforeHealth = target.Health;
        target.Heal(healingAmount);
        afterHealth = target.Health;
    }
}
public class HealTargetBuilder : IAbilityBuilder
{
    HealTargetAbility healTargetAbility = new HealTargetAbility();

    public Ability Build()
    {
        return healTargetAbility;
    }

    public IAbilityBuilder SetPerformer(GridEntity performer)
    {
        healTargetAbility.Performer = performer;
        return this;
    }

    public IAbilityBuilder SetTargetPosition(Vector3Int position)
    {
        return this;
    }

    public IAbilityBuilder SetRange(int range)
    {
        return this;
    }

    public IAbilityBuilder SetTargetEntity(GridEntity entity)
    {
        return this;
    }

    public IAbilityBuilder SetCard(Card card)
    {
        throw new NotImplementedException();
    }
}
#endregion Heal Target

#region Shield Target
[CreateAssetMenu(menuName = "Abilities/Heal Or Shield/Shield Action")]
public class ShieldTargetAbility : Ability
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

    public override void Perform()
    {
        base.Perform();
        Debug.Log($"{shielder.Armour} shields {target.name} for {armourAmount} health.");
        beforeArmour = target.Health;
        target.Shield(armourAmount);
        afterArmour = target.Armour;
    }
}

public class ShieldTrgetBuilder : IAbilityBuilder
{
    ShieldTargetAbility shieldTargetAbility = new ShieldTargetAbility();

    public Ability Build()
    {
        return shieldTargetAbility;
    }

    public IAbilityBuilder SetPerformer(GridEntity performer)
    {
        shieldTargetAbility.Performer = performer;
        return this;
    }

    public IAbilityBuilder SetTargetPosition(Vector3Int position)
    {
        return this;
    }

    public IAbilityBuilder SetRange(int range)
    {
        return this;
    }

    public IAbilityBuilder SetTargetEntity(GridEntity entity)
    {
        return this;
    }

    public IAbilityBuilder SetCard(Card card)
    {
        throw new NotImplementedException();
    }
}
#endregion Shield Target

#region Heal Group
[CreateAssetMenu(menuName = "Abilities/Heal Or Shield/Heal Group Action")]
public class HealGroupAbility : Ability
{
    [HideInInspector]
    public GridEntity healer;
    [HideInInspector]
    public List<GridEntity> targets;
    private Dictionary<GridEntity, int> beforeHealthDict;
    private Dictionary<GridEntity, int> afterHealthDict;
    public int healingAmount;
    public Vector3Int targetPosition;

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

    public override void Perform()
    {
        base.Perform();
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

public class HealGroupBuilder : IAbilityBuilder
{
    HealGroupAbility shieldTargetAbility = new HealGroupAbility();

    public Ability Build()
    {
        return shieldTargetAbility;
    }

    public IAbilityBuilder SetPerformer(GridEntity performer)
    {
        shieldTargetAbility.Performer = performer;
        return this;
    }

    public IAbilityBuilder SetTargetPosition(Vector3Int position)
    {
        return this;
    }

    public IAbilityBuilder SetRange(int range)
    {
        return this;
    }

    public IAbilityBuilder SetTargetEntity(GridEntity entity)
    {
        return this;
    }

    public IAbilityBuilder SetCard(Card card)
    {
        throw new NotImplementedException();
    }
}
#endregion Heal Group

#region Shield Group
[CreateAssetMenu(menuName = "Abilities/Heal Or Shield/Shield Group Action")]
public class ShieldGroupAbility : Ability
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

    public override void Perform()
    {
        base.Perform();
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

public class ShieldGroupBuilder : IAbilityBuilder
{
    ShieldGroupAbility shieldTargetAbility = new ShieldGroupAbility();

    public Ability Build()
    {
        return shieldTargetAbility;
    }

    public IAbilityBuilder SetPerformer(GridEntity performer)
    {
        shieldTargetAbility.Performer = performer;
        return this;
    }

    public IAbilityBuilder SetTargetPosition(Vector3Int position)
    {
        return this;
    }

    public IAbilityBuilder SetRange(int range)
    {
        return this;
    }

    public IAbilityBuilder SetTargetEntity(GridEntity entity)
    {
        return this;
    }

    public IAbilityBuilder SetCard(Card card)
    {
        throw new NotImplementedException();
    }
}
#endregion Shield Group

#endregion Healing / Shielding Abilities
*/