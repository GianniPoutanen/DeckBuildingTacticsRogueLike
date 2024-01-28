using System;
using System.Collections.Generic;
using UnityEngine;

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
                    instance.player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
                }
            }
            return instance;
        }
    }

    #endregion

    // Add your player-related variables and state here
    public int playerHealth = 100;
    public int maxEnergy = 2;
    public int currentEnergy = 2;

    // Example player object
    public PlayerController player;

    private void Start()
    {
        // You can perform any necessary initialization here
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        instance = null;
        UnsubscribeToEvents();
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


    #region Evemt Handlers
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
        currentEnergy = maxEnergy;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    #endregion Evemt Handlers
}
