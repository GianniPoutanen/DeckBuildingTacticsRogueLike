using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class PlayerManager : MonoBehaviour
{
    #region Singleton Pattern

    private static PlayerManager instance;

    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayerManager).Name;
                    instance = obj.AddComponent<PlayerManager>();
                    instance.Player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
                }
            }
            return instance;
        }
    }

    #endregion

    // Add your player-related variables and state here
    public int playerHealth = 100;
    public int maxEnergy = 2;
    private int _energy = 2;

    public PlayerStates State = PlayerStates.PlayerTurn;

    [SerializeField]
    public int CurrentEnergy
    {
        get { return _energy; }
        set
        {
            _energy = value;
            EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
        }
    }

    // Example player object
    public PlayerController Player;

    private void Start()
    {
        CurrentEnergy = maxEnergy;
        // You can perform any necessary initialization here
        Player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        SubscribeToEvents();
    }

    private void Update()
    {
        CheckEndTurn();
    }

    private void OnDestroy()
    {
        instance = null;
        UnsubscribeToEvents();
    }


    private void CheckEndTurn()
    {
        if (CurrentEnergy == 0 && State != PlayerStates.Waiting)
        {
            PlayerEndTurn();
        }
    }

    // Example method to handle when the player takes damage
    private void HandlePlayerDamageTaken(int damageAmount)
    {
        // Add logic to handle player damage
        playerHealth -= damageAmount;

        if (playerHealth <= 0)
        {
            // Player is defeated, perform game over logic or respawn logic here
            Debug.Log("Player defeated!");
        }
        else
        {
            Debug.Log($"Player took {damageAmount} damage. Remaining health: {playerHealth}");
        }
    }

    // Example method to apply damage to the player
    public void ApplyDamage(int damageAmount)
    {
        playerHealth -= damageAmount;
        // Invoke the PlayerDamageTaken event with the specified damage amount
        EventManager.Instance.InvokeEvent(Enums.EventType.PlayerDamageTaken, damageAmount);
    }


    public void PlayerEndTurn()
    {
        if (Player != null)
        {
            UndoRedoManager.Instance.AddUndoAction(new CompositeAction(Player.CurrentActions.ToList(), Player));
            Player.CurrentActions = new Stack<IUndoRedoAction>();
        }
        EventManager.Instance.InvokeEvent(Enums.EventType.EndPlayerTurn);
    }



    #region Event Handlers
    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(Enums.EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }
    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(Enums.EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }

    public void EndEnemyTurnHandler()
    {
        CurrentEnergy = maxEnergy;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    #endregion Event Handlers
}
