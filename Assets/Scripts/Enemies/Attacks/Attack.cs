using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attack
{
    [Header("Details")]
    public string attackName;
    public int cost= 1;
    [Header("The attack")]
    [SerializeField]
    public Ability triggerAbility;
    public List<Ability> followUpAbilities;
}