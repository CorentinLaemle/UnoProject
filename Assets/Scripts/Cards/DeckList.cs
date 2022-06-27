using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New deck", menuName = "Deck")]
public class DeckList : ScriptableObject //is used to create a list or cards which will be used as a deck by the game
{
    public List<Card> cards;
}
