using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnqueuAttackAction : Ability
{
    public Enemy enemy;
    public Attack attack;

    public EnqueuAttackAction(Enemy enemy, Attack attack)
    {
        Performer = enemy;
        this. enemy = enemy;
        this.attack = attack;
    }

    public override void Perform()
    {
        UndoRedoManager.Instance.AddUndoAction(this);
        AbilityBuilder abilityBuilder = AbilityBuilder.GetBuilder(attack.triggerAbility);
        enemy.attackQueue.Enqueue(abilityBuilder.SetTargetPosition(PlayerManager.Instance.Player.targetGridPosition).SetPerformer(Performer).Build());
        foreach (Ability ability in attack.followUpAbilities)
            enemy.attackQueue.Enqueue(AbilityBuilder.GetBuilder(ability).SetPerformer(Performer).SetTargetPosition(PlayerManager.Instance.Player.targetGridPosition).Build());
        GridManager.Instance.UpdateEnemyActionTiles();
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Undo()
    {
        enemy.attackQueue.Clear();
        GridManager.Instance.UpdateEnemyActionTiles();
    }
}
