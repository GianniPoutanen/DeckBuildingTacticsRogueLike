using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAttack
{
    public string attackName;
    [Header("The attack queue")]
    [SerializeField]
    public Ability triggerAbility;
    public List<Ability> followUpAbilities;
}