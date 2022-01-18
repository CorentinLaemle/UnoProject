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
    private RectTransform _myRectTransform;
    private bool _isCardInHand;
    private bool _isPlayerMainHand;
    private int _myPlayerIndex;

    private void Awake()
    {
        _isCardInHand = transform.parent.TryGetComponent(out HandManager handManager);
        if (_isCardInHand)
        {
            _myHandManager = handManager;
            _isPlayerMainHand = _myHandManager.IsMainPlayerHand;
            _myPlayerIndex = _myHandManager.MyPlayerIndex;
            _myHandAutoLayout = transform.parent.GetComponent<HandAutoLayout>();
            _myRectTransform = gameObject.GetComponent<RectTransform>();
        }
        _myButton = gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnActiveCardChanged += UpdateActiveCard;

        _myCardOutline.enabled = false;
        _myButton.enabled = false;

        UpdateActiveCard(GameManager.GetInstance().ActiveCard);
    }

    private void UpdateActiveCard(Card newActiveCard)
    {
        _currentActiveCard = newActiveCard;
        
        _isPlayable =
            _myCard._cardColor == Card.CardColor.black ||
            _myCard._cardColor == _currentActiveCard._cardColor  ||
            _myCard._cardValue == _currentActiveCard._cardValue ||
            _currentActiveCard._cardColor == Card.CardColor.black;

        _myButton.enabled = false;
        if (_isPlayerMainHand)
        {
            _myButton.enabled = _isPlayable;
            _myCardOutline.enabled = _isPlayable;
        }
    }

    //This method is called when a card is clicked on
    public void ClickOnCard()
    {
        if (_isCardInHand && GameManager.GetInstance().ActivePlayer == _myPlayerIndex)
        {
            IHaveBeenPlayed(_myCard);
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
    }
}
