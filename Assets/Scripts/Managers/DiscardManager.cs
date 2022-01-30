using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiscardAutoLayout))]
public class DiscardManager : MonoBehaviour
{
    public List<CardDisplay> _discardList;

    [SerializeField] private GameObject _cardPrefab;

    private DiscardAutoLayout _discardAutoLayout;
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
        
        _discardAutoLayout = gameObject.GetComponent<DiscardAutoLayout>();
    }
    private void Start()
    {
        CustomGameEvents.GetInstance().OnDistributeCardsEnded += DiscardCardFromDeck;
        CustomGameEvents.GetInstance().OnCardPlayed += DiscardCard;
    }

    //This method will be called by the deck manager when shuffling ; it will remove the card's dependencies then delete the attached gameobject
    public Card DeleteCardFromDiscard()
    {
        Card card = _discardList[0]._card;
        GameObject cardObject = _discardList[0].gameObject;

        _discardAutoLayout._cardsRectTransformList.RemoveAt(0);
        _discardList.RemoveAt(0);
        Destroy(cardObject);

        return card;
    }

    private void DiscardCardFromDeck() //Will only be called once, at the start of the game
    {
        GameObject newCard = Instantiate(_cardPrefab, transform);
        Card card = DeckManager.GetInstance().DrawOneCard();
        newCard.GetComponent<CardBehaviour>()._myCard = card;

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay._card = card;
        cardDisplay._isCardVisible = true;

        _discardList.Add(cardDisplay);
        _discardAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
        _discardAutoLayout.RepositionCardsInDiscard(0); //the parameter will dictate the orientation of the card. 0 means the card will face the player.

        CustomGameEvents.GetInstance().ActiveCardChanged(card);
        CustomGameEvents.GetInstance().GameStart();
    }

    private void DiscardCard(Card card, int playerIndex)
    {
        GameObject newCard = Instantiate(_cardPrefab, transform);

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        newCard.GetComponent<CardBehaviour>()._myCard = card;
        cardDisplay._card = card;
        cardDisplay._isCardVisible = true;

        _discardList.Add(cardDisplay);
        _discardAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
        _discardAutoLayout.RepositionCardsInDiscard(playerIndex);

        CustomGameEvents.GetInstance().CardPlayedAndDiscarded(card, playerIndex);
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnDistributeCardsEnded -= DiscardCardFromDeck;
        CustomGameEvents.GetInstance().OnCardPlayed -= DiscardCard;
    }
}
