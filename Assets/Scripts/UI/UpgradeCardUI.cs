using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour, IPointerClickHandler
{
    public Card card;

    // Set up the card with its name and click functionality
    public void SetupCard(Card card)
    {
        this.card = card;
    }

    // Method called when the card is clicked
    private void UpgradeCard()
    {
        // Print debug message indicating the card is being upgraded
        Debug.Log("Upgrading card: " + card.name);
        UpgradeCardAction upgradeCardAction = new UpgradeCardAction(card);
        UpgradeCardPanelSingleton.Instance.UpgradeSelected();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpgradeCard();
    }
}
