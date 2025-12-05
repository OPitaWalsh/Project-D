using System;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject cardPrefab;
    public Transform handTransform;
    public float fanSpread = 5f;
    public float horizontalSpacing = 5f;
    public float verticalSpacing = 5f;
    public List<GameObject> cardsInHand = new List<GameObject>();



    public void AddCardToHand(Card cardData)
    {
        //instantiates the card
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        newCard.GetComponent<CardDisplay>().cardData = cardData;
        cardsInHand.Add(newCard);
        newCard.GetComponent<CardStats>().state = CardStats.PlayState.InHand;

        UpdateHandVisuals();
    }


    private void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;

        for (int i = 0; i < cardCount; i++)
        {
            float rotationAngle = fanSpread * (i - (cardCount - 1) / 2f);
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            float horizontalOffset = horizontalSpacing * (i - (cardCount - 1) / 2f);
            float verticalOffset = verticalSpacing * Math.Abs(i - (cardCount - 1) / 2f);
            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
        }
    }
}
