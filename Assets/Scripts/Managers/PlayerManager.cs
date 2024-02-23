using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
                    instance.Player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
                }
            }
            return instance;
        }
    }

    #endregion

    // Add your player-related variables and state here
    [Header("Players Stats")]
    public int maxMovement = 2;
    public int numMovement = 2;

    [SerializeField]
    [Header("Decks")]
    public Deck playerDeck;
    [SerializeField]
    private Deck _activeDeck;
    [SerializeField]
    public Deck activeDeck
    {
        get
        {
            if (_activeDeck == null)
            {
                _activeDeck = new Deck();
                var newDeck =  Instantiate(instance.playerDeck);
                foreach (Card card in newDeck.Cards)
                    _activeDeck.AddCard(new Card(card));
                _activeDeck.Shuffle();
            }
            return _activeDeck;
        }
    }
    private Deck _discardPile;
    [SerializeField]
    public Deck discardPile
    {
        get
        {
            if (_discardPile == null)
                _discardPile = new Deck();
            return _discardPile;
        }
    }

    public PlayerStates State = PlayerStates.PlayerTurn;

    [SerializeField]
    public int CurrentEnergy
    {
        get { return Player.currentEnergy; }
        set
        {
            Player.currentEnergy = value;
            EventManager.Instance.InvokeEvent(EventType.UpdateUI);
        }
    }

    // Example player object
    public PlayerController Player;

    private void Start()
    {
        CurrentEnergy = Player.maxEnergy;
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
        Player.CurrentHeatlh -= damageAmount;

        if (Player.CurrentHeatlh <= 0)
        {
            // Player is defeated, perform game over logic or respawn logic here
            Debug.Log("Player defeated!");
        }
        else
        {
            Debug.Log($"Player took {damageAmount} damage. Remaining health: {Player.CurrentHeatlh}");
        }
    }

    // Example method to apply damage to the player
    public void ApplyDamage(int damageAmount)
    {
        Player.CurrentHeatlh -= damageAmount;
        // Invoke the PlayerDamageTaken event with the specified damage amount
        EventManager.Instance.InvokeEvent(EventType.PlayerAttacked, damageAmount);
    }


    public void PlayerEndTurn()
    {
        if (Player != null)
        {
            Player.CurrentActions = new Stack<Ability>();
        }
        State = PlayerStates.Waiting;
        EventManager.Instance.InvokeEvent(EventType.EndPlayerTurn);
    }



    #region Event Handlers
    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(EventType.GameStart, GameStartHandler);
        EventManager.Instance.AddListener(EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }
    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(EventType.EndEnemyTurn, EndEnemyTurnHandler);
        EventManager.Instance.RemoveListener(EventType.GameStart, GameStartHandler);
    }

    public void EndEnemyTurnHandler()
    {
        CurrentEnergy = Player.maxEnergy + Player.GetStatus(Status.Hasten);
        Player.Armour = 0;
        EventManager.Instance.InvokeEvent(EventType.UpdateUI);
    }

    public void GameStartHandler()
    {
        activeDeck.Shuffle();
    }

    #endregion Event Handlers
}
