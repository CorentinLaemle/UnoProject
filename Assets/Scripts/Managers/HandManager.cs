using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandAutoLayout))]
public class HandManager : MonoBehaviour
{
    public List<CardBehaviour> _cardsInHand;

    [SerializeField] [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] protected int _myPlayerIndex;
    [SerializeField] private bool _isMainPlayerHand;
    [SerializeField] private bool _isCurrentTurnActivePlayer;
    [SerializeField] private GameObject _cardPrefab;

    private bool _isForcedToDraw;
    private HandAutoLayout _handAutoLayout;
    private bool _hasDrawnThisTurn;

    public int MyPlayerIndex
    {
        get
        {
            return _myPlayerIndex;
        }
    }
    public bool IsMainPlayerHand
    {
        get
        {
            return _isMainPlayerHand;
        }
    }
    protected bool HasDrawnThisTurn
    {
        get
        {
            return _hasDrawnThisTurn;
        }
        set
        {
            _hasDrawnThisTurn = value;
        }
    }

    protected virtual void Awake()
    {
        _handAutoLayout = gameObject.GetComponent<HandAutoLayout>();
    }

    protected virtual void Start()
    {
        CustomGameEvents.GetInstance().OnTurnStart += StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected += PlayCard;
    }

    private void StartTurn(int playerIndex)
    {
        if(playerIndex != MyPlayerIndex)
        {
            _isCurrentTurnActivePlayer = false;
            return;
        }
        _isCurrentTurnActivePlayer = true;
        
        HasDrawnThisTurn = false;
        _isForcedToDraw = true;

        for(int i = 0 ; i < _cardsInHand.Count; i++)
        {
            if(_cardsInHand[i]._isPlayable == true)
            {
                _isForcedToDraw = false;
                break;
            }
        }
        if(_isForcedToDraw == true)
        {
            CustomGameEvents.GetInstance().PlayerMustDraw(); //Is listened by the DeckManager --> if called, the deckManager will enable its button and outline
        }
    }


    protected void ClickAndDraw() //used only to draw a card from the deck when forced to do so
    {
        if (_isCurrentTurnActivePlayer && !HasDrawnThisTurn)
        {
            HasDrawnThisTurn = true;

            if (!TryDrawCard(1))
            {
                HasDrawnThisTurn = false;
                ClickAndDraw();
                return;
            }
            return;
        }
        CustomGameEvents.GetInstance().PlayerHasSkipped();
    }

    public bool TryDrawCard(int cardNumber)
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
            Card card = DeckManager.GetInstance().DrawOneCard();

            CardBehaviour newCardBehaviour = newCard.GetComponent<CardBehaviour>();
            CardDisplay newCardDisplay = newCard.GetComponent<CardDisplay>();

            newCardBehaviour._myCard = card;
            newCardDisplay._card = card;
            newCardDisplay._isCardVisible = IsMainPlayerHand;

            _cardsInHand.Add(newCardBehaviour);

            //Populates the _rectTransformList with the corresponding components from the cards contained in the _cardsInHand list
            _handAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
            _handAutoLayout.RepositionCardsInHand();
        }
    }

    //on joue une carte en deux étapes : tout d'abord, en cliquant sur une carte on lance l'event OnCardSelected, qui est écouté par PlayCard
    //PlayCard va ensuite lancer RepositionCardsInHand + être écouté par le discard manager qui récupèrera la carte jouée depuis la main
    private void PlayCard(Card cardPlayed, int playerIndex)
    {
        if(playerIndex == MyPlayerIndex)
        {
            _handAutoLayout.RepositionCardsInHand();
            CustomGameEvents.GetInstance().CardPlayed(cardPlayed, MyPlayerIndex);
        }
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnStart -= StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected -= PlayCard;
    }
}
