using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    public List<CardDisplay> _discardList;
    [SerializeField] GameObject _cardPrefab;

    private CustomUIAutoLayout _discardAutoLayout;

    private static DiscardManager _instance;
    public static DiscardManager GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;
        
        _discardAutoLayout = GetComponent<CustomUIAutoLayout>();
    }
    private void Start()
    {
        CustomGameEvents.GetInstance().OnGameStart += DiscardCardFromDeck;
        CustomGameEvents.GetInstance().OnCardPlayed += DiscardCard;
    }

    //This method will be called by the deck manager when shuffling ; it will remove the card's dependencies, delete the attached gameobject
    public Card DeleteCardFromDiscard()
    {
        Card card = _discardList[0]._card;
        GameObject cardObject = _discardList[0].gameObject;

        if(_discardList.Count == _discardAutoLayout._cardsRectTransformList.Count) //so we don't get an error caused inderectly by the repositionCardsInDiscard method
        {
            _discardAutoLayout._cardsRectTransformList.RemoveAt(0);
        }
        _discardList.RemoveAt(0);
        Destroy(cardObject);

        return card;
    }

    private void DiscardCardFromDeck()
    {
        GameObject newCard = Instantiate(_cardPrefab, transform);
        newCard.GetComponent<CardDisplay>()._card = DeckManager.GetInstance().DrawOneCard();
        newCard.GetComponent<CardDisplay>()._isCardVisible = true;

        CardDisplay card = newCard.GetComponent<CardDisplay>();
        _discardList.Add(card);
        _discardAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
        _discardAutoLayout.RepositionCardsInDiscard(0); //the parameter will dictate the orientation of the card. 0 means the card will face the player.

        CustomGameEvents.GetInstance().ActiveCardChanged(card._card);
    }

    private void DiscardCard(Card card, int playerIndex)
    {
        GameObject newCard = Instantiate(_cardPrefab, transform);
        newCard.GetComponent<CardDisplay>()._card = card;
        newCard.GetComponent<CardDisplay>()._isCardVisible = true;

        _discardList.Add(newCard.GetComponent<CardDisplay>());
        _discardAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
        _discardAutoLayout.RepositionCardsInDiscard(playerIndex);

        CustomGameEvents.GetInstance().ActiveCardChanged(card);
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnGameStart -= DiscardCardFromDeck;
        CustomGameEvents.GetInstance().OnCardPlayed -= DiscardCard;
    }
}
