using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> allCards = new List<Card>();
    private int currentIndex = 0;

    void Start()
    {
        //load all card assets from the Resources folder
        Card[] cards = Resources.LoadAll<Card>("Cards");

        //add loaded cards to allCards list
        allCards.AddRange(cards);
    }


    public void DrawCard(HandManager handManager)
    {
        if (allCards.Count == 0) { return; }

        Card nextCard = allCards[currentIndex];
        handManager.AddCardToHand(nextCard);
        currentIndex = (currentIndex + 1) % allCards.Count;     //wrap around if end of deck reached but not out of cards
    }
}
