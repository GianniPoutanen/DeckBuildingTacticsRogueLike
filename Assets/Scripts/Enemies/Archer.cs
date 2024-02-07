using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    public int StraightAttackRange;
    public StraightAttackAbility StraightAttack;

    public override void AfterTurnCheckCanAttack()
    {
        base.AfterTurnCheckCanAttack();

        var entitiesInStraightAttackRange = GridManager.Instance.GetEntitiesOnPositions(GridManager.Instance.GetPositionsInDistance(this.targetGridPosition, StraightAttackRange, false));

        // Attack #1
        if (entitiesInStraightAttackRange.Contains(PlayerManager.Instance.Player))
        {
            new StraightAttackBuilder();
        }
    }
}
