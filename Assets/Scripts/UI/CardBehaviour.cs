using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private Card _currentActiveCard;
    [SerializeField] private Image _myCardOutline;

    public bool _isPlayable;

    private CardDisplay _myCardDisplay;
    private Button _myButton;
    private HandAutoLayout _myHandAutoLayout;
    private HandManager _myHandManager;
    private RectTransform _myRectTransform;
    private bool _isCardInHand;
    private bool _isPlayerMainHand;
    private int _myPlayerIndex;
    private bool _isMyTurn;

    private void Awake()
    {
        _myCardDisplay = gameObject.GetComponent<CardDisplay>();
        _myButton = gameObject.GetComponent<Button>();

        _isCardInHand = transform.parent.TryGetComponent(out HandManager handManager);
        if (_isCardInHand)
        {
            _myHandManager = handManager;
            _isPlayerMainHand = _myHandManager._isMainPlayerHand;
            _myPlayerIndex = _myHandManager._myPlayerIndex;
            _myHandAutoLayout = transform.parent.GetComponent<HandAutoLayout>();
            _myRectTransform = gameObject.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnActiveCardChanged += UpdateActiveCard;
        CustomGameEvents.GetInstance().OnTurnStart += CheckActiveTurnPlayer;

        _myCardOutline.enabled = false;
        _myButton.enabled = false;
    }

    private void UpdateActiveCard(Card newActiveCard)
    {
        _currentActiveCard = newActiveCard;
        
        _isPlayable =
            _myCardDisplay._card._cardColor == Card.CardColor.black ||
            _myCardDisplay._card._cardColor == _currentActiveCard._cardColor  ||
            _myCardDisplay._card._cardValue == _currentActiveCard._cardValue;

        _myButton.enabled = false;
        if (_isPlayerMainHand)
        {
            _myButton.enabled = _isPlayable;
            _myCardOutline.enabled = _isPlayable;
        }
    }

    public void CheckActiveTurnPlayer(int activePlayerIndex)
    {
        _isMyTurn = activePlayerIndex == _myPlayerIndex;
    }

    //This method is called when a card is clicked on
    //Attention : this condition could be deleted, depending on which rules we play with. In that case, I'll need to  modify the way turn currently end
    //todo : this method will be called by the AI at it's turn, IF the chosen card can be played + is the best (?) option
    public void ClickOnCard()
    {
        if (_isCardInHand && _isMyTurn) 
        {
            IHaveBeenPlayed(_myCardDisplay._card);
        }
    }

    private void IHaveBeenPlayed(Card card)
    {
        for(int i = 0; i < _myHandAutoLayout._cardsRectTransformList.Count; i++)
        {
            if(_myHandAutoLayout._cardsRectTransformList[i] == _myRectTransform)
            {
                _myHandAutoLayout._cardsRectTransformList.RemoveAt(i);
                _myHandManager._cardsInHand.RemoveAt(i);
                Destroy(gameObject);
                break;
            }
        }
        CustomGameEvents.GetInstance().CardSelected(card, _myPlayerIndex);
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnActiveCardChanged -= UpdateActiveCard;
        CustomGameEvents.GetInstance().OnTurnStart -= CheckActiveTurnPlayer;
    }
}
