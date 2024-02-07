using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Decks/Deck")]
[Serializable]
public class Deck : ScriptableObject
{
    [SerializeField]
    public List<Card> Cards = new List<Card>();

    // Shuffle the deck using Fisher-Yates algorithm
    public void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = Cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = Cards[k];
            Cards[k] = Cards[n];
            Cards[n] = value;
        }

        EventManager.Instance.InvokeEvent<Deck>(Enums.EventType.DeckShuffled, this);
    }

    // Draw a card from the deck
    public Card DrawCard()
    {
        if (Cards.Count > 0)
        {
            Card drawnCard = Cards[0];
            Cards.RemoveAt(0);
            return drawnCard;
        }
        else
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }
    }

    public void Insert(Card card, int index)
    {
        Cards.Insert(0,card);
    }

    // Add a card to the deck
    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    // Add multiple cards to the deck
    public void AddCards(List<Card> newCards)
    {
        Cards.AddRange(newCards);
    }
}