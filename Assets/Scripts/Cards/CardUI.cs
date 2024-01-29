using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public bool dragging = false;

    public Vector2 followPosition;
    public Vector2 targetLocalPosition; // Target position is now local to the parent
    public Card card;

    private RectTransform rectTransform;
    private Vector2 originalLocalPosition;
    [Header("UI Elemetns")]
    public TextMeshProUGUI tmpText;

    [Header("Positioning and Movement")]
    public float moveSpeed = 5f; // Adjust this value based on your preference


    private void Start()
    {
        originalLocalPosition = (Vector2)transform.localPosition;
        rectTransform = GetComponent<RectTransform>();
        tmpText.text = card.cardName;
    }

    private void Update()
    {
        // Interpolate the current local position towards the target local position
        Vector2 newLocalPosition = Vector2.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * moveSpeed);
        transform.localPosition = newLocalPosition;

        float distanceToMouse = Vector2.Distance(transform.position, Input.mousePosition);
        this.rectTransform.localPosition += new Vector3(0, 0, (Mathf.RoundToInt(-distanceToMouse) / 50f) + (UIManager.Instance.hand.cardUnderMouse == this ? -10f : 0f));
    }

    public void OnDrag(PointerEventData eventData)
    {
        EventManager.Instance.InvokeEvent(Enums.EventType.CardStartDragging, this);
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if the card is dropped within a valid target area
        if (!UIManager.Instance.hand.UIUnderMouse)
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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Optionally handle click or tap events
    }

    private void TryPlayCard()
    {
        // Check if the card can be played
        if (card != null && card.CanPlay())
        {
            // Perform actions to play the card
            card.Play();
            EventManager.Instance.InvokeEvent(Enums.EventType.CardPlayed, card);
            UIManager.Instance.hand.cardsInHand.Remove(this);
            Destroy(this.gameObject);
            // Optionally, you can implement additional logic here
            // for example, removing the card from the player's hand.
        }
    }
}
