using UnityEngine;

public class CardStats : MonoBehaviour
{
    [SerializeField] private DiscardManager discardManager;
    private Card cardData;
    public enum PlayState
    {
        InDeck,
        InHand,
        InPlay,
        InDiscard
    }

    public PlayState state = PlayState.InDeck;


    void Awake()
    {
        discardManager = FindFirstObjectByType<DiscardManager>();
    }


    public void EnterPlay()
    {
        cardData = GetComponent<CardDisplay>().cardData;
        state = PlayState.InPlay;
    }

    public void TakeDamage(int amount)
    {
        cardData.health -= amount;
        GetComponent<CardDisplay>().UpdateCardDisplay();
        if (cardData.health <= 0)
        {
            state = PlayState.InDiscard;
            discardManager.AddToDiscard(cardData);
            Destroy(gameObject);
        }
    }
}
