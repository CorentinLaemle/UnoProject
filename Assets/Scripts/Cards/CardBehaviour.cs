using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    public bool _isPlayable;
    public Card _myCard;

    [SerializeField] private Card _currentActiveCard;
    [SerializeField] private Image _myCardOutline;
    [SerializeField] private Button _myButton;

    private HandAutoLayout _myHandAutoLayout;
    private HandManager _myHandManager;
    private bool _isCardInHand;
    private bool _isPlayerMainHand;
    private int _myPlayerIndex;

    private void Awake()
    {
        _isCardInHand = transform.parent.parent.TryGetComponent(out HandManager handManager);
        if (_isCardInHand)
        {
            _myHandManager = handManager;
            _isPlayerMainHand = _myHandManager.IsMainPlayerHand;
            _myPlayerIndex = _myHandManager.MyPlayerIndex;
            _myHandAutoLayout = transform.parent.parent.GetComponent<HandAutoLayout>();
        }
        _myButton = gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnTurnStart += UpdateActiveCard;
        CustomGameEvents.GetInstance().OnActiveCardChanged += UpdateActiveCard;

        _myCardOutline.enabled = false;
        _myButton.interactable = false;

        UpdateActiveCard(UnoGameMaster.GetInstance().ActivePlayer);
    }

    private void UpdateActiveCard(Card card)
    {
        UpdateActiveCard(UnoGameMaster.GetInstance().ActivePlayer);
    }

    private void UpdateActiveCard(int playerIndex)
    {
        _currentActiveCard = UnoGameMaster.GetInstance().ActiveCard;

        _isPlayable =
        _myCard._cardColor == Card.CardColor.black ||
        _myCard._cardColor == _currentActiveCard._cardColor  ||
        _myCard._cardValue == _currentActiveCard._cardValue ||
        _currentActiveCard._cardColor == Card.CardColor.black;

        _myButton.interactable = false;
        _myCardOutline.enabled = false;

        if (_isPlayerMainHand && playerIndex == _myPlayerIndex)
        {
            _myButton.interactable = _isPlayable;
            _myCardOutline.enabled = _isPlayable;
        }
    }

    //This method is called when a card is clicked on
    public void ClickOnCard()
    {
        if (_isCardInHand && UnoGameMaster.GetInstance().ActivePlayer == _myPlayerIndex)
        {
            IHaveBeenPlayed(_myCard);
        }
    }

    private void IHaveBeenPlayed(Card card) 
    {
        for(int i = 0; i < _myHandManager._cardsInHand.Count; i++)
        {
            if(_myHandManager._cardsInHand[i] == this)
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
        CustomGameEvents.GetInstance().OnTurnStart -= UpdateActiveCard;
        CustomGameEvents.GetInstance().OnActiveCardChanged -= UpdateActiveCard;
    }
}
