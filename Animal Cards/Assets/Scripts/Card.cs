using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite cardImage;
    public List<CardType> cardType;
    public int health;
    public int damage;

    public enum CardType
    {
        Feral,
        Tame
    }
}