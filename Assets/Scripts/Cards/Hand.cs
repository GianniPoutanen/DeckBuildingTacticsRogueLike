using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [Header("Cards")]
    public GameObject cardPrefab;
    public List<CardUI> cardsInHand = new List<CardUI>();
    public float cardSpacing = 150f;

    [Header("Cards Positioning Variables")]
    public Vector2 initialPosition = new Vector2(0f, 0f);
    public Vector2 draggingOffset = new Vector2(0f, 50f);
    public float cursorFloatingYAmount = 50f;

    [Header("Selection Under Mouse")]
    public bool UIUnderMouse = false;
    public CardUI cardUnderMouse;

    [Header("Parent Variables")]
    public GraphicRaycaster raycaster;

    // While dragging or playing
    public bool draggingCard = false;

    private void Awake()
    {
        SubscribeToEvents();
    }

    // Example usage:
    void Start()
    {
        // Assuming you have a Card instance named "exampleCard"
        Card exampleCard = new Card();
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
        SpawnCard(exampleCard);
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for the card under the mouse during drag
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Set up the new Pointer Event
            var pointerEventData = new PointerEventData(EventSystem.current);
            //Set the Pointer Event Position to that of the game object
            pointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            raycaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                UIUnderMouse = true;
                foreach (RaycastResult result in results)
                {
                    Debug.Log(result.distance);
                    if (result.gameObject.GetComponent<CardUI>() != null )
                    {
                        CardUI hitCard = result.gameObject.GetComponent<CardUI>();
                        if (hitCard != null && cardsInHand.Contains(hitCard))
                        {
                            cardUnderMouse = hitCard;
                            UpdateCardPositions();
                        }
                    }
                }
            }
            else
            {
                UIUnderMouse = false;
            }
        }
        else
        {
            cardUnderMouse = null;
            UpdateCardPositions();
        }

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

            // If a card is under the mouse, move other cards out of the way
            if (cardUnderMouse != null && cardUnderMouse == cardsInHand[i])
            {
                newPosition.y += cursorFloatingYAmount; // Adjust this value based on your design
            }

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

        // Set the sibling index to reorder the children based on their z values
        for (int i = 0; i < sortedChildren.Length; i++)
        {
            sortedChildren[i].SetSiblingIndex(i);
        }
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

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener<CardUI>(Enums.EventType.CardStartDragging, CardStartDraggingHandler);
        EventManager.Instance.AddListener<CardUI>(Enums.EventType.CardEndDragging, CardEndDraggingHandler);
    }
    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener<CardUI>(Enums.EventType.CardStartDragging, CardStartDraggingHandler);
        EventManager.Instance.RemoveListener<CardUI>(Enums.EventType.CardEndDragging, CardEndDraggingHandler);
    }

    #endregion Event Handlers
}
