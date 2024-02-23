using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnqueuAttackAction : Ability
{
    public EnemyAlly entity;
    public Attack attack;

    public EnqueuAttackAction(EnemyAlly entity, Attack attack)
    {
        Performer = entity;
        this. entity = entity;
        this.attack = attack;
    }

    public override void Perform()
    {
        UndoRedoManager.Instance.AddUndoAction(this);
        AbilityBuilder abilityBuilder = AbilityBuilder.GetBuilder(attack.triggerAbility);
        entity.attackQueue.Enqueue(abilityBuilder.SetTargetPosition(TargetPosition).SetPerformer(Performer).Build());
        foreach (Ability ability in attack.followUpAbilities)
            entity.attackQueue.Enqueue(AbilityBuilder.GetBuilder(ability).SetPerformer(Performer).SetTargetPosition(entity.currentTarget.targetGridPosition).Build());
        GridManager.Instance.UpdateEnemyActionTiles();
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Undo()
    {
        entity.attackQueue.Clear();
        GridManager.Instance.UpdateEnemyActionTiles();
    }
}
