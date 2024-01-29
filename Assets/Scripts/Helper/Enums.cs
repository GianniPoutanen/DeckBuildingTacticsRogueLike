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
        Entitydestroyed,

        // Enemy 
        EndEnemyTurn,
        UpdateUI,
    }
    public enum PlayerStates
    {
        Waiting,
        PlayerTurn,
    }

}
