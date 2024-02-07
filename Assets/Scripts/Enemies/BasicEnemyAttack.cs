using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BasicEnemyAttack : IEnemyAttack
{
    public int attackDamage = 3;
    public int range = 1;
    public bool rotateAble = true;

    [SerializeField]
    public List<Ability> abilities = new List<Ability>();

    public BasicEnemyAttack(List<Ability> abilities)
    {
        //IAbilityBuilder abilityBuilder = AbilityBuilder.GetBuilder new EnemyAttackBase()
        foreach (Ability ability in abilities)
        {
            this.abilities.Add(ability);
        }
    }

    public bool CanPerform(Vector3 position)
    {
        return false;
    }
    public void ActionAttack()
    {

    }
    public List<Vector3Int> GetEffectivePositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        return positions;
    }
}
