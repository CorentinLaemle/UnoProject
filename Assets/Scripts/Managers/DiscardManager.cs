using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    public List<CardDisplay> _discardList;

    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private AudioSource _source;

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

        AudioManager.GetInstance().SetAudioSource(_source, "PLAYCARD");
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

        CardBehaviour cardBehaviour = newCard.GetComponent<CardBehaviour>();
        cardBehaviour.enabled = false;

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay._card = card;
        cardDisplay._isCardVisible = true;

        _discardList.Add(cardDisplay);
        _discardAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
        _discardAutoLayout.RepositionCardsInDiscard(0); //the parameter will dictate the orientation of the card. 0 means the card will face the player.

        PlaySound();

        CustomGameEvents.GetInstance().ActiveCardChanged(card);
        CustomGameEvents.GetInstance().GameStart();
    }

    private void DiscardCard(Card card, int playerIndex)
    {
        GameObject newCard = Instantiate(_cardPrefab, transform);
        
        CardBehaviour cardBehaviour = newCard.GetComponent<CardBehaviour>();
        cardBehaviour.enabled = false;

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay._card = card;
        cardDisplay._isCardVisible = true;

        _discardList.Add(cardDisplay);
        _discardAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
        _discardAutoLayout.RepositionCardsInDiscard(playerIndex);

        PlaySound();

        CustomGameEvents.GetInstance().CardPlayedAndDiscarded(card, playerIndex);
    }

    private void PlaySound()
    {
        AudioManager.GetInstance().PlayPitchedSound(_source);
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnDistributeCardsEnded -= DiscardCardFromDeck;
        CustomGameEvents.GetInstance().OnCardPlayed -= DiscardCard;
    }
}
