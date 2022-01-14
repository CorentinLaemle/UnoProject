using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandAutoLayout))]
public class HandManager : MonoBehaviour
{
    [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] public int _myPlayerIndex;
    public bool _isCurrentTurnActivePlayer;

    public bool _isMainPlayerHand;
    [SerializeField] private GameObject _cardPrefab;

    public List<CardDisplay> _cardsInHand;

    [HideInInspector] public HandAutoLayout _handAutoLayout;

    private void Awake()
    {
        _handAutoLayout = GetComponent<HandAutoLayout>();
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnTurnStart += StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected += PlayCard;
        CustomGameEvents.GetInstance().OnTurnEnd += EndTurn;
    }

    private void StartTurn(int playerIndex)
    {
        if(playerIndex == _myPlayerIndex)
        {
            _isCurrentTurnActivePlayer = true;
        }
    }

    private void EndTurn()
    {
        _isCurrentTurnActivePlayer = false;
    }

    public bool CallDrawCard(int cardNumber)
    {
        //Checks if there are enough cards left in the deck for the requested amount of cards. If false, the deck is shuffled.
        if (DeckManager.GetInstance().CheckDrawPossible(cardNumber))
        {
            DrawCards(cardNumber);
            return true;
        }
        return false;
    }

    private void DrawCards(int cardNumber)
    {
        for (int i = 0; i < cardNumber; i++)
        {
            GameObject newCard = Instantiate(_cardPrefab, transform);
            CardDisplay newCardDisplay = newCard.GetComponent<CardDisplay>();
            newCardDisplay._card = DeckManager.GetInstance().DrawOneCard();
            //If this card is part of the main player's hand, its isCardVisible bool is set to true
            newCardDisplay._isCardVisible = _isMainPlayerHand;
                
            //We need to add the card to the _cardsInHand list BEFORE calling the RepositionCardsInHand method, otherwize everything breaks.
            _cardsInHand.Add(newCardDisplay);
            //Populates the _rectTransformList with the corresponding components from the cards contained in the _cardsInHand list
            //We do this here since all further operations will need it, and this method is only called once per draw
            _handAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
            _handAutoLayout.RepositionCardsInHand();
        }
    }

    //on joue une carte en deux étapes : tout d'abord, en cliquant sur une carte on lance l'event OnCardSelected, qui est écouté par PlayCard
    //PlayCard va ensuite lancer RepositionCardsInHand + être écouté par le discard manager qui récupèrera la carte jouée depuis la main
    private void PlayCard(Card cardPlayed, int playerIndex)
    {
        if(playerIndex == _myPlayerIndex)
        {
            _handAutoLayout.RepositionCardsInHand();
            CustomGameEvents.GetInstance().CardPlayed(cardPlayed, _myPlayerIndex);
        }
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnStart -= StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected -= PlayCard;
        CustomGameEvents.GetInstance().OnTurnEnd -= EndTurn;
    }
}
