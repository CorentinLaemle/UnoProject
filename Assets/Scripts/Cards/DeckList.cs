using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New deck", menuName = "Deck")]
public class DeckList : ScriptableObject
{
    public List<Card> cards;
}
