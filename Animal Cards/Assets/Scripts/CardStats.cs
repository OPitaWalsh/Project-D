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
    public int currHP;


    void Awake()
    {
        discardManager = gameObject.GetComponent<DiscardManager>();
        cardData = gameObject.GetComponent<CardDisplay>().cardData;
    }


    public void EnterPlay()
    {
        state = PlayState.InPlay;
        currHP = cardData.health;
    }

    public void TakeDamage(int amount)
    {
        currHP -= amount;
        if (currHP <= 0)
        {
            state = PlayState.InDiscard;
            discardManager.AddToDiscard(cardData);
        }
    }
}
