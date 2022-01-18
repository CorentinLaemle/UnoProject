using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandAutoLayout))]
public class HandManager : MonoBehaviour
{
    public List<GameObject> _cardsInHand;

    [SerializeField] [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] private int _myPlayerIndex;
    [SerializeField] private bool _isMainPlayerHand;
    [SerializeField] private bool _isCurrentTurnActivePlayer;
    [SerializeField] private GameObject _cardPrefab;

    private bool _isForcedToDraw;
    private HandAutoLayout _handAutoLayout;
    private bool _hasDrawnThisTurn;

    public bool HasDrawnThisTurn
    {
        get
        {
            return _hasDrawnThisTurn;
        }
        private set
        {
            _hasDrawnThisTurn = value;
        }
    }

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

    private void Awake()
    {
        _handAutoLayout = gameObject.GetComponent<HandAutoLayout>();
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnTurnStart += StartTurn;
        CustomGameEvents.GetInstance().OnCardSelected += PlayCard;
        CustomGameEvents.GetInstance().OnTurnEnd += EndTurn;
    }

    private void StartTurn(int playerIndex)
    {
        if(playerIndex == MyPlayerIndex)
        {
            _isCurrentTurnActivePlayer = true;
        }
        HasDrawnThisTurn = false;
        _isForcedToDraw = true;

        if (_isCurrentTurnActivePlayer)
        {
            for(int i = 0 ; i < _cardsInHand.Count; i++)
            {
                if(_cardsInHand[i].GetComponent<CardBehaviour>()._isPlayable == true)
                {
                    _isForcedToDraw = false;
                    break;
                }
            }

            if(_isForcedToDraw == true)
            {
                CustomGameEvents.GetInstance().PlayerMustDraw();
            }
        }
    }

    private void EndTurn()
    {
        _isCurrentTurnActivePlayer = false;
    }

    public void ClickAndDraw() //used only to draw a card from the deck when forced to do so
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
        if (_isCurrentTurnActivePlayer)
        {
            CustomGameEvents.GetInstance().PlayerHasSkipped();
        }
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
            _cardsInHand.Add(newCard);

            Card card = DeckManager.GetInstance().DrawOneCard();

            newCard.GetComponent<CardBehaviour>()._myCard = card;
            newCard.GetComponent<CardDisplay>()._card = card;
            newCard.GetComponent<CardDisplay>()._isCardVisible = IsMainPlayerHand;

            //Populates the _rectTransformList with the corresponding components from the cards contained in the _cardsInHand list
            _handAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
            _handAutoLayout.RepositionCardsInHand();
        }
    }

    //on joue une carte en deux �tapes : tout d'abord, en cliquant sur une carte on lance l'event OnCardSelected, qui est �cout� par PlayCard
    //PlayCard va ensuite lancer RepositionCardsInHand + �tre �cout� par le discard manager qui r�cup�rera la carte jou�e depuis la main
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
        CustomGameEvents.GetInstance().OnTurnEnd -= EndTurn;
    }
}
