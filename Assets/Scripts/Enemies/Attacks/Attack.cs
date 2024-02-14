using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attack
{
    [Header("Details")]
    public string attackName;
    [Header("The attack")]
    [SerializeField]
    public Ability triggerAbility;
    public List<Ability> followUpAbilities;
}