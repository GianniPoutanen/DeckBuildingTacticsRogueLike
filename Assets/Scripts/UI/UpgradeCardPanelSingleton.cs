using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UpgradeCardPanelSingleton : UIElement
{
    [Header("Upgrade Stats")]
    public int numCards;
    public GameObject upgradeCardUI;

    #region Singleton Pattern
    private static UpgradeCardPanelSingleton instance;

    public static UpgradeCardPanelSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UpgradeCardPanelSingleton>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(UpgradeCardPanelSingleton).Name;
                    instance = obj.AddComponent<UpgradeCardPanelSingleton>();
                }
            }
            return instance;
        }
    }
    #endregion Singleton Pattern

    public override void Start()
    {
        base.Start();
        this.gameObject.SetActive(false);
        UIManager.Instance.upgradeCardPanel = this;
    }

    public void InitiateUpgrade()
    {
        int possibleUpgradeAmount = numCards < PlayerManager.Instance.activeDeck.Cards.Count ? PlayerManager.Instance.activeDeck.Cards.Count : numCards;
        this.gameObject.SetActive(true);
        List<Card> cardOptions = PlayerManager.Instance.activeDeck.Cards;
        List<Card> upgradeableCards = new List<Card>();
        for (int i = 0; i < possibleUpgradeAmount; i++)
        {
            Card card = cardOptions[Random.Range(0, cardOptions.Count)];
            cardOptions.Remove(card);
            upgradeableCards.Add(card);
        }

        // TODO add panel increasing
        int upgradeIndex = 0;
        foreach (Card card in upgradeableCards)
        {
            UpgradeCardUI newUpgrade = GameObject.Instantiate(upgradeCardUI).GetComponent<UpgradeCardUI>();
            newUpgrade.SetupCard(card);
            RectTransform rectTransform = newUpgrade.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * upgradeIndex, 0);
            upgradeIndex++;
        }
    }

    public void UpgradeSelected()
    {
        Debug.Log("Upgraded Finished");
        this.gameObject.SetActive(false);
    }
}