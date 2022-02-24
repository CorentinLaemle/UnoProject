using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card _card;
    public bool _isCardVisible;
    public ColorIndex _colorIndex;

    [SerializeField] private Image _cardValueChild;
    [SerializeField] private Image _cardColorChild;
    [SerializeField] private Sprite _cardBackSideSprite;

    private float _nextStep;

    void Start()
    {
        _nextStep = Time.time;
    }

    private void Update()
    {
        if(_nextStep < Time.time)
        {
            DisplayCard(_isCardVisible);
            _nextStep += 1;
        }
    }

    private void DisplayCard(bool isCardVisible)
    {
        if (isCardVisible)
        {
            _cardValueChild.sprite = _card._sprite;

            switch (_card._cardColor)
            {
                case Card.CardColor.black:
                    _cardColorChild.color = _colorIndex._cardBlack;
                    break;
                case Card.CardColor.blue:
                    _cardColorChild.color = _colorIndex._cardBlue;
                    break;
                case Card.CardColor.green:
                    _cardColorChild.color = _colorIndex._cardGreen;
                    break;
                case Card.CardColor.red:
                    _cardColorChild.color = _colorIndex._cardRed;
                    break;
                case Card.CardColor.yellow:
                    _cardColorChild.color = _colorIndex._cardYellow;
                    break;
                default:
                    Debug.Log("CardDisplay error : the selected color doesn't exist");
                    break;
            }
        }
        else
        {
            _cardValueChild.sprite = _cardBackSideSprite;
            _cardColorChild.color = _colorIndex._transparency;
        }
    }
}
