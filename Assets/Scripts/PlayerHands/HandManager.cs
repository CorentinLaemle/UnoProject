using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public List<CardBehaviour> _cardsInHand;

    [SerializeField] [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] protected int _myPlayerIndex;
    [SerializeField] private bool _isMainPlayerHand;
    [SerializeField] private bool _isCurrentTurnActivePlayer;
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _cardsObjectsParent;
    [SerializeField] private AudioSource _source;

    private bool _isForcedToDraw;
    private HandAutoLayout _handAutoLayout;
    private bool _hasDrawnThisTurn;
    private bool _hasPlayedThisTurn;

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
    public bool HasPlayedThisTurn
    {
        get
        {
            return _hasPlayedThisTurn;
        }
        set
        {
            _hasPlayedThisTurn = value;
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

        AudioManager.GetInstance().SetAudioSource(_source, "DRAWCARD");
    }

    private void StartTurn(int playerIndex)
    {
        if(playerIndex != MyPlayerIndex)
        {
            _isCurrentTurnActivePlayer = false;
            return;
        }
        _isCurrentTurnActivePlayer = true;

        HasPlayedThisTurn = false;
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
            PlaySound();
            return true;
        }
        return false;
    }

    private void DrawCards(int cardNumber)
    {
        for (int i = 0; i < cardNumber; i++)
        {
            GameObject newCard = Instantiate(_cardPrefab, _cardsObjectsParent.transform);
            Card card = DeckManager.GetInstance().DrawOneCard();

            CardBehaviour newCardBehaviour = newCard.GetComponent<CardBehaviour>();
            CardDisplay newCardDisplay = newCard.GetComponent<CardDisplay>();

            newCardBehaviour._myCard = card;
            newCardDisplay._card = card;
            newCardDisplay._isCardVisible = IsMainPlayerHand;

            int cardIndex = InsertCardInHand(newCardBehaviour);
            newCard.transform.SetSiblingIndex(cardIndex);

            //Populates the _rectTransformList with the corresponding components from the cards contained in the _cardsInHand list
            if(_handAutoLayout._cardsRectTransformList.Count < cardIndex)
            {
                _handAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
                _handAutoLayout.RepositionCardsInHand();
                return;
            }
            _handAutoLayout._cardsRectTransformList.Insert(cardIndex, newCard.GetComponent<RectTransform>());
            _handAutoLayout.RepositionCardsInHand();
        }
    }

    private int InsertCardInHand(CardBehaviour cardBehaviour)
    {
        int index = 0;
        bool isIndexFound = false;

        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            if (_cardsInHand[i]._myCard._cardColor == cardBehaviour._myCard._cardColor)
            {
                if (_cardsInHand[i]._myCard._cardValue >= cardBehaviour._myCard._cardValue)
                {
                    index = i;
                    isIndexFound = true;
                    break;
                }
            }
            if((int)_cardsInHand[i]._myCard._cardColor > (int)cardBehaviour._myCard._cardColor)
            {
                index = i;
                isIndexFound = true;
                break;
            }
            index = i;
        }

        if (isIndexFound)
        {
            _cardsInHand.Insert(index, cardBehaviour);
            return index;
        }
        _cardsInHand.Add(cardBehaviour);
        return index+1;
    }

    //on joue une carte en deux �tapes : tout d'abord, en cliquant sur une carte on lance l'event OnCardSelected, qui est �cout� par PlayCard
    //PlayCard va ensuite lancer RepositionCardsInHand + �tre �cout� par le discard manager qui r�cup�rera la carte jou�e depuis la main
    private void PlayCard(Card cardPlayed, int playerIndex)
    {
        if(playerIndex == MyPlayerIndex)
        {
            HasPlayedThisTurn = true;
            _handAutoLayout.RepositionCardsInHand();
            CustomGameEvents.GetInstance().CardPlayed(cardPlayed, MyPlayerIndex);
        }
    }

    private void PlaySound()
    {
        AudioManager.GetInstance().PlayPitchedSound(_source);
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnStart -= StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected -= PlayCard;
    }
}
