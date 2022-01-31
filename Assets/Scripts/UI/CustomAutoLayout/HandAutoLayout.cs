using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HandManager))]
public class HandAutoLayout : CustomAutoLayout
{
    [SerializeField] private enum CustomLayoutType
    {
        top,
        left,
        bottom,
        right,
    }
    [SerializeField] private CustomLayoutType _myLayoutType;
    [SerializeField] private bool _isLeftHandedMode;
    [SerializeField] private int _startCardsInterval;
    [SerializeField] private int _spaceDecrementStep;
    [SerializeField][Range(-1,1)] private float _alpha;
    [SerializeField][Range(0,1)] private float _delta;
    [SerializeField] private float _maxCardRotation;

    private int _trueAlpha;
    private int _trueDelta;
    private HandManager _myHandManager;

    protected override void Awake()
    {
        base.Awake();
        _myHandManager = gameObject.GetComponent<HandManager>();
    }

    private void Start()
    {
        _trueAlpha = (int) (_alpha * _cardPrefab.GetComponent<RectTransform>().rect.height);
        _trueDelta = (int) (_delta * _cardPrefab.GetComponent<RectTransform>().rect.width);
    }

    //This method is called by the corresponding HandManager whenever it draws a card
    public void RepositionCardsInHand()
    {
        _cardsNumber = _myHandManager._cardsInHand.Count;

        //Depending on the selected CustomLayoutType, we call the corresponding method with the following parameters :
        //Rotation of the cards, anchor point (min and max are the same), and the direction used to place the cards in hand (from the 1st to the last)
        switch (_myLayoutType)
        {
            case CustomLayoutType.bottom:
                RepositionCards(Vector3.zero, new Vector2(0, 0.5f), Vector2.right);
                break;
            case CustomLayoutType.left:
                //RepositionCards(new Vector3(0, 0, -90), new Vector2(0.5f, 1), Vector2.down);
                RepositionCards(Vector3.zero, new Vector2(0.5f, 1), Vector2.down);
                break;
            case CustomLayoutType.top:
                RepositionCards(new Vector3(0, 0, 180), new Vector2(1, 0.5f), Vector2.left);
                break;
            case CustomLayoutType.right:
                //RepositionCards(new Vector3(0, 0, 90), new Vector2(0.5f, 0), Vector2.up);
                RepositionCards(Vector3.zero, new Vector2(0.5f, 0), Vector2.up);
                break;
            default:
                Debug.Log("HandAutoLayout error : the selected CustomLayoutType doesn't exist.");
                break;
        }
    }

    private void RepositionCards(Vector3 cardRotation, Vector2 anchors, Vector2 nextCardIncrement)
    {
        float rotX = 0f;
        float rotY = 0f;
        float rotZ = 0f;

        //This line opens the possibility of changing the way cards appear in the hand. 
        //If _isLeftHandedMode is false (default), the last drawn card will be placed on the rigth of the hand.
        nextCardIncrement *= _isLeftHandedMode ? -1 : 1;

        //calculates the rought number of pixels we have at our disposal to place all the cards. We cast as an int since anchors may have float values.
        int handAreaSize = (int)(Screen.height * (_rectTransform.anchorMax.y - _rectTransform.anchorMin.y));
        if (nextCardIncrement == Vector2.left || nextCardIncrement == Vector2.right)
        {
            handAreaSize = (int)(Screen.width * (_rectTransform.anchorMax.x - _rectTransform.anchorMin.x));
        }

        int cardsInHand = _cardsNumber;
        //we only want to space out our cards if there is more than one card to space out
        int spaceBetweenCards = (cardsInHand > 1) ? _startCardsInterval : 0;
        //since we rotate each card to be facing the center of the screen, we'll always use the card width to determine the space taken by one card 
        int cardWidth = (int)_cardRectTransform.rect.width;

        //determines if we can place all the cards in hand using the current spaceBetweenCards parameter
        bool isEnoughSpace = handAreaSize > cardWidth * cardsInHand;

        if (!isEnoughSpace)
        {
            while (!isEnoughSpace)
            {
                //if not, recalculates by decrementing spaceBetweenCards
                spaceBetweenCards -= _spaceDecrementStep;
                isEnoughSpace = handAreaSize > (cardWidth + spaceBetweenCards) * cardsInHand;
            }
        }

        //once we corrected the spaceBetween cards parameter so all cards in hand can visually fit inside of the hand area, we place them
        if (nextCardIncrement == Vector2.left || nextCardIncrement == Vector2.right) //for the top and bottom players
        {
            for (int i = 0; i < cardsInHand; i++)
            {
                int correctX = (handAreaSize / (cardsInHand + 1)) * (i + 1) - handAreaSize / 2;
                if (correctX == 0) //We add this line in order to avoid errors when recognizing cards in the hand when played. Should be invisible by eye
                {
                    correctX = 1;
                }
                correctX *= (int)nextCardIncrement.x;

                float beta = Mathf.PI * i/ (cardsInHand -1);
                int correctY = (int)(_trueAlpha + Mathf.Sin(beta) * _trueDelta);

                if(nextCardIncrement == Vector2.left) //todo : reprendre avec une variable + un calcul de la rotation en fonction du nombre de cartes en main
                {
                    rotX = -90;
                }
                _maxCardRotation = 5 * (cardsInHand-1) / 2;
                rotZ = (int)_maxCardRotation * nextCardIncrement.x - 2*_maxCardRotation * i/cardsInHand * nextCardIncrement.x;
                Vector3 correctRotation = new Vector3(cardRotation.x + rotX, cardRotation.y + rotY, cardRotation.z + rotZ);

                _cardsRectTransformList[i].anchorMin = anchors;
                _cardsRectTransformList[i].anchorMax = anchors;
                _cardsRectTransformList[i].localPosition = new Vector3(correctX, correctY);
                _cardsRectTransformList[i].rotation = Quaternion.Euler(correctRotation);
            }
            return;
        }
        for (int i = 0; i < cardsInHand; i++) //for the left and right players
        {
            int correctY = (handAreaSize / (cardsInHand + 1)) * (i + 1) - handAreaSize / 2;
            if (correctY == 0)
            {
                correctY = 1;
            }
            correctY *= (int)nextCardIncrement.y;

            float beta = Mathf.PI * i/ (cardsInHand - 1);
            int correctX = (int)(_trueAlpha + Mathf.Sin(beta) * _trueDelta * -nextCardIncrement.y);

            _cardsRectTransformList[i].anchorMin = anchors;
            _cardsRectTransformList[i].anchorMax = anchors;
            _cardsRectTransformList[i].localPosition = new Vector3(correctX, correctY);
            _cardsRectTransformList[i].rotation = Quaternion.Euler(cardRotation);
        }
    }
}
