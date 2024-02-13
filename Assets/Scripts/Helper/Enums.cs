using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    // Game Related
    GameStart,

    // Player
    PlayerAttacked,
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
    EntityTurn,
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

public enum JsonDataType
{
    DeckData,
    GameSettings,
    PlayerStats,
}

public enum UnlockType
{
    CharacterA,
}

public enum UIPanels
{
    AttackPanel
}

public enum AbilityTypes
{
    BasicAttack,
    StraightAttack,
    SurroundingAttack,
    MoveTarget,
}