using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
    public enum EventType
    {
        PlayerDamageTaken,
        EntitySpawned,
        Entitydestroyed,
        EndEnemyTurn,
        EndPlayerTurn,
        TurnTickComplete,
        UpdateUI
    }
}
