using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New card", menuName = "Card")]
public class Card : ScriptableObject
{
    public enum cardColor
    {
        red,
        yellow,
        blue,
        green,
        black
    }

    //Paramètres communs à toutes les cartes
    [Tooltip("For number cards, this field correspond to their value. Reverse = 10, skip = 11 and draw two = 12. For black cards, use 13 for color change and 14 for +4.")]
    public int _cardValue;
    public Sprite _sprite;
    public cardColor _cardColor;
}
