using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class CardDisplay : MonoBehaviour
{
    public Image backgroundImage;
    public Card cardData;
    public Image cardImage;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text damageText;
    public Image[] typeImages;

    private Color[] cardColors =
    {
        new Color(0.44f, 0.22f, 0.0f), //feral
        new Color(0.0f, 0.22f, 0.44f) //tame
    };



    void Start()
    {
        UpdateCardDisplay();
    }



    public void UpdateCardDisplay()
    {
        //update the main card image color based on the first card type
        backgroundImage.color = cardColors[(int)cardData.cardType[0]];
        //update type images
        for (int i = 0; i < typeImages.Length; i++)
        {
            if (i < cardData.cardType.Count)
            {
                typeImages[i].gameObject.SetActive(true);
            } else
            {
                typeImages[i].gameObject.SetActive(false);
            }
        }

        //update text
        nameText.text = cardData.cardName;
        healthText.text = cardData.health.ToString();
        damageText.text = cardData.damage.ToString();

        //update animal image
        cardImage.sprite = cardData.cardImage;
    }
}