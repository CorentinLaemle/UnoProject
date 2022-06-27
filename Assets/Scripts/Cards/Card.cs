using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New card", menuName = "Card")]
public class Card : ScriptableObject
{
    //Card parameters

    public enum CardColor
    {
        red,
        yellow,
        blue,
        green,
        black,
        invalid
    }

    [Tooltip("For number cards, this field correspond to their value. Reverse = 10, skip = 11 and draw two = 12. For black cards, use 13 for color change and 14 for +4. The value 15 is used when we want to forbid any player from playing a card.")]
    [Range(0,15)] public int _cardValue;
    public Sprite _sprite;
    public CardColor _cardColor;
}
