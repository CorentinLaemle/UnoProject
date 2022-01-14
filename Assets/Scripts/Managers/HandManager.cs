using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandAutoLayout))]
public class HandManager : MonoBehaviour
{
    [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] public int _myPlayerIndex;
    [SerializeField] private bool _isCurrentTurnActivePlayer;

    public bool _isMainPlayerHand;
    private bool _isForcedToDraw;
    [SerializeField] private GameObject _cardPrefab;

    public List<GameObject> _cardsInHand;

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
                CustomGameEvents.GetInstance().PlayerMustDraw(_myPlayerIndex);
            }
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

    public void ClickAndDraw() //used only by the main player to physically draw a card when forced to do so
    {
        if (_isMainPlayerHand)
        {
            DrawCards(1);
            CustomGameEvents.GetInstance().PlayerHasDrawnAndSkipped();
        }
    }

    private void DrawCards(int cardNumber)
    {
        for (int i = 0; i < cardNumber; i++)
        {
            GameObject newCard = Instantiate(_cardPrefab, transform);
            _cardsInHand.Add(newCard);

            CardDisplay newCardDisplay = newCard.GetComponent<CardDisplay>();
            newCardDisplay._card = DeckManager.GetInstance().DrawOneCard();
            //If this card is part of the main player's hand, its isCardVisible bool is set to true
            newCardDisplay._isCardVisible = _isMainPlayerHand;

            //Populates the _rectTransformList with the corresponding components from the cards contained in the _cardsInHand list
            //We do this here since all further operations will need it, and this method is only called once per draw
            _handAutoLayout._cardsRectTransformList.Add(newCard.GetComponent<RectTransform>());
            _handAutoLayout.RepositionCardsInHand();
        }
    }

    //on joue une carte en deux �tapes : tout d'abord, en cliquant sur une carte on lance l'event OnCardSelected, qui est �cout� par PlayCard
    //PlayCard va ensuite lancer RepositionCardsInHand + �tre �cout� par le discard manager qui r�cup�rera la carte jou�e depuis la main
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
