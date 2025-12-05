using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckManager : MonoBehaviour
{
    public List<Card> deckCards = new List<Card>();
    private DiscardManager discardManager;
    private HandManager handManager;
    public TextMeshProUGUI deckCardCounter;

    private int currentIndex = 0;
    public int startHandSize;


    void Awake()
    {
        if (discardManager == null)
        {
            discardManager = FindFirstObjectByType<DiscardManager>();
        }

        if (handManager == null)
        {
            handManager = FindFirstObjectByType<HandManager>();
        }
    }
    
    
    void Start()
    {
        //load all card assets from the Resources folder
        Card[] cards = Resources.LoadAll<Card>("Cards");
        //add loaded cards to deckCards list
        deckCards.AddRange(cards);

        GameManager.Instance.ResetGame();
    }


    private void UpdateDeckCount()
    {
        deckCardCounter.text = deckCards.Count.ToString();
    }


    public void ResetGame()
    {
        //return discard to deck
        deckCards.AddRange(discardManager.PullAllFromDiscard());

        UpdateDeckCount();

        Shuffle();

        //draw starting hand
        for (int i = 0; i < startHandSize; i++)
        {
            DrawCard();
        }
        GameManager.Instance.IncreasePhaseCount();
    }


    public void Shuffle()
    {
        System.Random random = new System.Random();
        int n = deckCards.Count;
        for (int i = n-1; i > 0; i--)
        {
            int j = random.Next(i+1);
            (deckCards[j], deckCards[i]) = (deckCards[i], deckCards[j]);    //swap card i wih a random card
        }
    }


    public void DrawCard()
    {
        if (deckCards.Count == 0) { return; }

        Card nextCard = deckCards[currentIndex];
        handManager.AddCardToHand(nextCard);
        deckCards.RemoveAt(currentIndex);
        if (deckCards.Count > 0)
        {
            currentIndex %= deckCards.Count;    //wrap around if end of deck reached but not out of cards
        }    
        
        UpdateDeckCount();
    }


    public void DrawCardButton()
    {
        if (GameManager.Instance.TurnCount == 1 && GameManager.Instance.PhaseCount == 0)
        {
            DrawCard();
            GameManager.Instance.IncreasePhaseCount();
        }
    }

    public void EndPhaseButton()
    {
        if (GameManager.Instance.TurnCount == 1 && GameManager.Instance.PhaseCount == 1)
        {
            GameManager.Instance.IncreasePhaseCount();
        }
    }

    public void EndTurnButton()
    {
        if (GameManager.Instance.TurnCount == 1 && GameManager.Instance.PhaseCount == 2)
        {
            GameManager.Instance.IncreasePhaseCount();
            GameManager.Instance.IncreaseTurnCount();
            GameManager.Instance.IncreaseTurnCount();       ////DEBUG: there's only one player, so skip p2's turn
        }
    }
}
