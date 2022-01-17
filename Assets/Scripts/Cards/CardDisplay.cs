using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card _card;
    [SerializeField] private Image _cardValueChild;
    [SerializeField] private Sprite _cardBackSideImage;

    public ColorIndex _colorIndex;

    private Image _cardColorImage;
    private float _nextStep;
    public bool _isCardVisible;

    private void Awake()
    {
        _cardColorImage = gameObject.GetComponent<Image>();
    }

    void Start()
    {
        _nextStep = Time.time;
    }

    private void Update()
    {
        if(_nextStep < Time.time)
        {
            DisplayCard(_card, _isCardVisible);
            _nextStep += 1;
        }
    }

    private void DisplayCard(Card activeCard, bool isCardVisible)
    {
        if (isCardVisible)
        {
            _cardValueChild.sprite = activeCard._sprite;

            switch (activeCard._cardColor)
            {
                case Card.CardColor.black:
                    _cardColorImage.color = _colorIndex._cardBlack;
                    break;
                case Card.CardColor.blue:
                    _cardColorImage.color = _colorIndex._cardBlue;
                    break;
                case Card.CardColor.green:
                    _cardColorImage.color = _colorIndex._cardGreen;
                    break;
                case Card.CardColor.red:
                    _cardColorImage.color = _colorIndex._cardRed;
                    break;
                case Card.CardColor.yellow:
                    _cardColorImage.color = _colorIndex._cardYellow;
                    break;
                default:
                    Debug.Log("CardDisplay error : the selected color doesn't exist");
                    break;
            }
        }
        else
        {
            _cardValueChild.sprite = _cardBackSideImage;
            _cardColorImage.color = _colorIndex._transparency;
        }
    }
}
