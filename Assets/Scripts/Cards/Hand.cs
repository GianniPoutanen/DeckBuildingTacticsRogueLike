using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private enum HandState
    {
        Idle,
        Focused,
        PlayingCard
    }
    private HandState state;
    public bool mouseInHand;
    public int handSizeLimit = 4;

    [Header("Cards")]
    public GameObject cardPrefab;
    public List<CardUI> cardsInHand = new List<CardUI>();
    public CardUI cardBeingPlayed;

    public RectTransform cardSpawnPosition;
    private Deck PlayerDeck { get { return PlayerManager.Instance.activeDeck; } }
    private Deck DiscardPile { get { return PlayerManager.Instance.discardPile; } }

    [Header("Cards Positioning Variables")]
    public Vector2 initialPosition = new Vector2(0f, 0f);
    public Vector2 draggingOffset = new Vector2(0f, 50f);
    public float cursorFloatingYAmount = 50f;
    public float idleCardYSinkAmount = 50f;
    public float cardSpacing = 150f;

    [Header("Selection Under Mouse")]
    public bool UIUnderMouse = false;
    public CardUI cardUnderMouse;


    // While dragging or playing
    public bool draggingCard = false;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        SubscribeToEvents();
    }

    // Example usage:
    void Start()
    {
        DrawCardsToHand(2);
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCardPositions();
        SetState();
        SetHandWidth();
    }

    public void SetHandWidth()
    {
        this.rectTransform.sizeDelta = new Vector2((cardPrefab.GetComponent<RectTransform>().sizeDelta * cardsInHand.Count + new Vector2(50f, 0)).x, this.rectTransform.sizeDelta.y);
    }

    public void SetState()
    {
        if (cardBeingPlayed != null)
        {
            state = HandState.PlayingCard;
        }
        else if (mouseInHand)
        {
            state = HandState.Focused;
        }
        else
        {
            state = HandState.Idle;
        }
    }

    public void DrawCardsToHand(int numCards)
    {
        for (int i = 0; i < numCards; i++)
        {
            DrawCardToHand();
        }
    }

    // Example: Call this method to draw a card from the deck
    public void DrawCardToHand()
    {
        if (cardsInHand.Count < handSizeLimit)
        {
            if (PlayerDeck.Cards.Count > 0)
            {
                DrawCardFromPlayerDeck();
            }
            else if (DiscardPile.Cards.Count > 0)
            {
                PlayerDeck.AddCards(DiscardPile.Cards);
                PlayerDeck.Shuffle();
                DiscardPile.Cards.Clear();
                DrawCardFromPlayerDeck();
            }
            else
            {
                Debug.Log("Failed to draw a card. Deck and discard pile are empty.");
            }
        }
        else
        {
            Debug.Log("Hand limit reached!");
        }
    }

    private void DrawCardFromPlayerDeck()
    {
        Card drawnCard = PlayerDeck.DrawCard();

        if (drawnCard != null)
        {
            SpawnCard((drawnCard));
            Debug.Log("Drew a card: " + drawnCard.cardName);
        }
        else
        {
            Debug.Log("Failed to draw a card.");
        }
        UndoRedoManager.Instance.AddUndoAction(new DrawCardAction(drawnCard));
    }

    public void RemoveCard(Card card)
    {
        CardUI obj = this.cardsInHand.Find(x => x.card.Equals(card));
        this.cardsInHand.Remove(obj);
        Destroy(obj.gameObject);
    }

    public void SpawnCard(Card card)
    {
        // Instantiate a new CardUI prefab
        GameObject cardObject = Instantiate(cardPrefab, transform);

        // Set the card's position in the hand
        Vector2 newPosition = CalculateCardPosition(cardsInHand.Count);
        cardObject.GetComponent<RectTransform>().anchoredPosition = newPosition;

        // Set the target position for the card
        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.targetLocalPosition = newPosition;

        // Set the card data
        cardUI.card = card;
        cardUI.sizeFactor = UIManager.Instance.CardSpawnSize;

        if (cardSpawnPosition != null)
            cardUI.transform.position = cardSpawnPosition.position;

        // Add the card to the list
        cardsInHand.Add(cardUI);
    }

    private Vector2 CalculateCardPosition(int cardIndex)
    {
        // Calculate the position of the card in the hand based on the index
        float totalWidth = cardsInHand.Count * cardSpacing;
        float startX = (-totalWidth / 2f) + (cardSpacing / 2);
        return new Vector2(startX + cardIndex * cardSpacing, 0f) + (draggingCard ? draggingOffset : Vector2.zero);
    }

    // Call this function when a card is played or removed from the hand
    public void UpdateCardPositions()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector2 newPosition = CalculateCardPosition(i);

            if (state == HandState.Idle)
                newPosition += new Vector2(0, -idleCardYSinkAmount);
            if (cardUnderMouse != null && cardUnderMouse == cardsInHand[i])
                newPosition.y += cursorFloatingYAmount;

            cardsInHand[i].targetLocalPosition = newPosition;
        }
        SortCards();
    }


    void SortCards()
    {
        // Get all child transforms
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        // Sort child transforms by their local z position
        Transform[] sortedChildren = children.OrderBy(child => child.localPosition.z).ToArray();

        if (cardBeingPlayed != null)
        {
            // Set the sibling index to reorder the children based on their z values
            for (int i = 0; i < cardsInHand.Count(); i++)
            {
                cardsInHand[i].transform.SetSiblingIndex(i);
            }
            cardBeingPlayed.transform.SetAsLastSibling();
        }
        else
        {
            if (state == HandState.Focused)
            {
                // Set the sibling index to reorder the children based on their z values
                for (int i = 0; i < sortedChildren.Length; i++)
                {
                    sortedChildren[i].SetSiblingIndex(i);
                }
            }
            else
            {
                for (int i = 0; i < cardsInHand.Count(); i++)
                {
                    cardsInHand[i].transform.SetSiblingIndex(i);
                }
            }

        }

        if (cardUnderMouse != null)
            cardUnderMouse.transform.SetAsLastSibling();
    }

    #region Event Handlers

    public void CardStartDraggingHandler(CardUI entity)
    {
        draggingCard = true;
    }

    public void CardEndDraggingHandler(CardUI entity)
    {
        draggingCard = false;
    }

    public void CardPlayedHandler(Card card)
    {
        SortCards();
        UpdateCardPositions();
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener<CardUI>(EventType.CardStartDragging, CardStartDraggingHandler);
        EventManager.Instance.AddListener<CardUI>(EventType.CardEndDragging, CardEndDraggingHandler);
        EventManager.Instance.AddListener<Card>(EventType.CardPlayed, CardPlayedHandler);
        EventManager.Instance.AddListener(EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }
    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener<CardUI>(EventType.CardStartDragging, CardStartDraggingHandler);
        EventManager.Instance.RemoveListener<CardUI>(EventType.CardEndDragging, CardEndDraggingHandler);
        EventManager.Instance.RemoveListener<Card>(EventType.CardPlayed, CardPlayedHandler);
        EventManager.Instance.RemoveListener(EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }
    public void EndEnemyTurnHandler()
    {
        DrawCardToHand();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseInHand = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseInHand = false;
    }

    #endregion Event Handlers
}
