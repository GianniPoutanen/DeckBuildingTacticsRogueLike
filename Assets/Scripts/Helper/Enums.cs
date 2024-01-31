using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
    public enum EventType
    {
        // Player
        PlayerDamageTaken,
        TurnTickComplete,

        CardStartDragging,
        CardEndDragging,
        CardPlayed,

        EndPlayerTurn,
        //Entity
        EntitySpawned,
        EntityDestroyed,

        // Enemy 
        EndEnemyTurn,
        UpdateUI,
    }
    public enum PlayerStates
    {
        Waiting,
        PlayerTurn,
    }

    public enum CastType
    {
        Simple,
        WithinDistance,
        Cone,
        Area,
        // Add more cast types if needed
    }
}
