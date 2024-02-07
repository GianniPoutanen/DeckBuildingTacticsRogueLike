using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : UIElement, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    public bool dragging = false;

    public Vector2 followPosition;
    public Vector2 targetLocalPosition; // Target position is now local to the parent
    public Card card;

    private Vector2 originalLocalPosition;
    [Header("UI Elemetns")]
    public TextMeshProUGUI tmpText;

    [Header("Positioning and Movement")]
    public float moveSpeed = 5f; // Adjust this value based on your preference

    [Header("Sizing Elememts")]
    private Vector2 originalSize;
    public float currentSizeFactor;

    private List<Vector3Int> castSelectionLocation = new List<Vector3Int>();


    public override void Start()
    {
        base.Start();
        originalLocalPosition = (Vector2)transform.localPosition;
        tmpText.text = card.cardName;
        originalSize = RectTransform.sizeDelta;
    }

    public override void Update()
    {
        base.Update();
        // Interpolate the current local position towards the target local position
        Vector2 newLocalPosition = Vector2.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * moveSpeed);
        transform.localPosition = newLocalPosition;

        float distanceToMouse = Vector2.Distance(transform.position, Input.mousePosition);
        RectTransform.localPosition += new Vector3(0, 0, (Mathf.RoundToInt(-distanceToMouse) / 50f) + (UIManager.Instance.Hand.cardUnderMouse == this ? -10f : 0f));
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIManager.Instance.Hand.cardBeingPlayed = this;
        EventManager.Instance.InvokeEvent(Enums.EventType.CardStartDragging, this);
        HighlightCardCastLocations();
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UIManager.Instance.Hand.cardBeingPlayed = null;
        // Check if the card is dropped within a valid target area
        if (!UIManager.Instance.Hand.UIUnderMouse)
        {
            TryPlayCard();
        }
        else
        {
            // Return the card to its original local position if not dropped in a valid area
            transform.localPosition = originalLocalPosition;
        }
        dragging = false;
        EventManager.Instance.InvokeEvent(Enums.EventType.CardEndDragging, this);
        castSelectionLocation.Clear();
        GridManager.Instance.UpdateCastPositionsTilemap(castSelectionLocation);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Optionally handle click or tap events
    }

    private void HighlightCardCastLocations()
    {
        if (card != null && card.abilities.Length > 0 && card.range > 0 ? card.CanPlay(GridManager.Instance.GetGridPositionFromWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))) : card.CanPlay(PlayerManager.Instance.Player.targetGridPosition))
        {
            castSelectionLocation.Clear();

            foreach (AbilityWrapper wrapper in card.abilities)
                castSelectionLocation.AddRange(wrapper.ability.GetAbilityPositions());
            GridManager.Instance.UpdateCastPositionsTilemap(castSelectionLocation);
        }
        else
        {
            castSelectionLocation.Clear();
            GridManager.Instance.UpdateCastPositionsTilemap(castSelectionLocation);
        }
    }

    private void TryPlayCard()
    {

        // Check if the card can be played
        if (card != null && card.abilities.Length > 0 && card.range > 0 ? card.CanPlay(GridManager.Instance.GetGridPositionFromWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))) : card.CanPlay(PlayerManager.Instance.Player.targetGridPosition))
        {
            // Perform actions to play the card
            card.Play();
            EventManager.Instance.InvokeEvent(Enums.EventType.CardPlayed, card);
            UIManager.Instance.Hand.cardsInHand.Remove(this);
            Destroy(this.gameObject);
            // Optionally, you can implement additional logic here
            // for example, removing the card from the player's hand.
        }
        else
        {
            Debug.Log("Couldn't play card");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIManager.Instance.Hand.cardUnderMouse == this)
            UIManager.Instance.Hand.cardUnderMouse = null;

        this.targetSizeFactor = 1f;
    }
}
