using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
    public enum EventType
    {
        // Game Related
        GameStart,

        // Player
        PlayerDamageTaken,
        TurnTickComplete,

        CardStartDragging,
        CardEndDragging,
        CardPlayed,

        DeckShuffled,

        EndPlayerTurn,
        //Entity
        EntitySpawned,
        EntityDestroyed,

        // Enemy 
        EndEnemyTurn,
        UpdateUI,
        AttackQueued,
        AttackDequeued,
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
