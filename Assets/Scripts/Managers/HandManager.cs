using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] public int _myPlayerIndex;
    public bool _isCurrentTurnActivePlayer;

    public bool _isMainPlayerHand;
    [SerializeField] private GameObject _cardPrefab;

    public List<CardDisplay> _cardsInHand;

    [HideInInspector] public CustomUIAutoLayout _handAutoLayout;
    private float _delayBetweenTryDraws;
    private float _turnBeginTimeMarker;
    private float _turnTime;

    private void Awake()
    {
        _handAutoLayout = GetComponent<CustomUIAutoLayout>();
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnTurnBegin += StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected += PlayCard;

        _turnTime = GameManager.GetInstance()._turnTime;
        _delayBetweenTryDraws = GameManager.GetInstance()._delayBetweenTryDraws;
    }

    private void Update()
    {
        float turnTimeLimit = _turnBeginTimeMarker + _turnTime;

        if(_isCurrentTurnActivePlayer && turnTimeLimit < Time.time)
        {
            EndTurn(_myPlayerIndex);
        }
    }

    //Gets called indirectly by the GameManager script, through the customGameEvents script
    private void StartTurn(int playerIndex)
    {
        if(playerIndex == _myPlayerIndex)
        {
            _turnBeginTimeMarker = Time.time;
            _isCurrentTurnActivePlayer = true;
        }
    }
    //Gets called at the end of the player's turn. 
    //The info will be processed by the GameManager script, in order to determine the next player and begin his/her turn
    private void EndTurn(int playerIndex)
    {
        _isCurrentTurnActivePlayer = false;
        CustomGameEvents.GetInstance().TurnEnd();
    }

    public bool CallDrawCard(int cardNumber)
    {
        //Checks if there is enough cards left in the deck for the requested amount of cards. If false, the deck is shuffled.
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
            _isCurrentTurnActivePlayer = false;

            CustomGameEvents.GetInstance().CardPlayed(cardPlayed, _myPlayerIndex);
        }
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnBegin -= StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected -= PlayCard;
    }
}
