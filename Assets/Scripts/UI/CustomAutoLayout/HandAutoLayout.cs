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
    [SerializeField] private int _defaultPixelsBetweenCards;
    [SerializeField] private int _cardsGapDecrementStep;
    [SerializeField][Range(-1,1)] private float _fanCenterGap;
    [SerializeField][Range(0,10)] private float _fanCurveIntensity;
    [SerializeField][Range(-10,10)] private float _cardRotPerCardInHand;
    [SerializeField] private RectTransform _zoomedInHandArea;

    private float _trueFanGap;
    private float _trueFanCurve;

    private HandManager _myHandManager;
    private RectTransform _activeHandArea;

    private bool _isExpanded;

    protected override void Awake()
    {
        base.Awake();
        _myHandManager = gameObject.GetComponent<HandManager>();
    }

    private void Start()
    {
        _activeHandArea = _rectTransform;
        _isExpanded = false;
    }

    public void ExpandCardArea() //this method is called from a button click. A simple cheat to better see the cards in the main player's hand
    {
        if(_isExpanded == false)
        {
            _isExpanded = true;
            _activeHandArea = _zoomedInHandArea;
            RepositionCardsInHand();
            return;
        }
        _isExpanded = false;
        _activeHandArea = _rectTransform;
        RepositionCardsInHand();
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
        int correctZ = _cardHeight / 2; //since the cards are tilted towards the screen, we need to move them away from their hand area in order to avoid clipping
        _trueFanGap = _fanCenterGap * _cardHeight;
        _trueFanCurve = _fanCurveIntensity * _cardWidth / 100;

        //calculates the rought number of pixels we have at our disposal to place all the cards. We cast as an int since anchors may have float values.
        int handAreaSize = (int)(Screen.height * (_activeHandArea.anchorMax.y - _activeHandArea.anchorMin.y));
        if (nextCardIncrement == Vector2.left || nextCardIncrement == Vector2.right)
        {
            handAreaSize = (int)(Screen.width * (_activeHandArea.anchorMax.x - _activeHandArea.anchorMin.x));
        }

        int cardsInHand = _cardsNumber;
        //we only want to space out our cards if there is more than one card to space out
        int spaceBetweenCards = (cardsInHand > 1) ? _defaultPixelsBetweenCards : 0;

        //since we rotate each card to be facing the center of the screen, we'll always use the card width to determine the space taken by one card 
        //determines if we can place all the cards in hand using the current spaceBetweenCards parameter
        int bufferArea = cardsInHand * _cardWidth + (cardsInHand - 1) * spaceBetweenCards;
        
        bool isEnoughSpace = handAreaSize >= bufferArea;
        if (isEnoughSpace)
        {
            handAreaSize = bufferArea;
        }

        if (!isEnoughSpace)
        {
            while (!isEnoughSpace)
            {
                //if not, recalculates by decrementing spaceBetweenCards
                spaceBetweenCards -= _cardsGapDecrementStep;
                bufferArea = cardsInHand * _cardWidth + (cardsInHand - 1) * spaceBetweenCards;

                isEnoughSpace = handAreaSize > bufferArea;
            }
            handAreaSize = bufferArea;
        }

        //once we corrected the spaceBetween cards parameter so all cards in hand can visually fit inside of the hand area, we place them
        if (nextCardIncrement == Vector2.left || nextCardIncrement == Vector2.right) //for the top and bottom players
        {
            for (int i = 0; i < cardsInHand; i++)
            {
                int correctX = handAreaSize / (cardsInHand + 1) * (i + 1) - handAreaSize / 2;
                correctX *= (int)nextCardIncrement.x;

                float beta = Mathf.PI * (i+1)/(cardsInHand+1); //if there's only one card in had it's placed in the middle, otherwise they are placed following a sine curve
                int correctY =  (int)(_trueFanGap + Mathf.Sin(beta) * _trueFanCurve * cardsInHand);

                rotX = -55; //value obtained through testing
                float maxCardRotation = _cardRotPerCardInHand * (cardsInHand-1) / 2;
                rotZ = maxCardRotation * nextCardIncrement.x - 2*maxCardRotation * (i+1)/(cardsInHand+1) * nextCardIncrement.x;
                Vector3 correctRotation = new Vector3(cardRotation.x + rotX, cardRotation.y + rotY, cardRotation.z + rotZ);

                _cardsRectTransformList[i].anchorMin = anchors;
                _cardsRectTransformList[i].anchorMax = anchors;
                _cardsRectTransformList[i].localPosition = new Vector3(correctX, correctY, -correctZ) ;
                _cardsRectTransformList[i].rotation = Quaternion.Euler(correctRotation);
            }
            return;
        }
        for (int i = 0; i < cardsInHand; i++) //for the left and right players
        {
            float beta = Mathf.PI * (i+1)/(cardsInHand+1);
            float curveFactor = Mathf.Sin(beta) * _trueFanCurve;

            int correctY = (handAreaSize / (cardsInHand + 1)) * (i + 1) - handAreaSize / 2;
            correctY *= (int)-nextCardIncrement.y;

            int correctX = (int)(_trueFanGap + curveFactor * cardsInHand * -nextCardIncrement.y);

            float maxCardRotation = _cardRotPerCardInHand * (cardsInHand - 1) / 2;
            rotX = 0 /*maxCardRotation * -nextCardIncrement.y + _cardRotPerCardInHand * curveFactor * nextCardIncrement.y*/ ; //todo : P1(nI.y =-1) : + vers - ; P3 : - vers +
            rotY = 65 * -nextCardIncrement.y;
            rotZ = 90 * nextCardIncrement.y;
            Vector3 correctRotation = new Vector3(cardRotation.x + rotX, cardRotation.y + rotY, cardRotation.z + rotZ);

            _cardsRectTransformList[i].anchorMin = anchors;
            _cardsRectTransformList[i].anchorMax = anchors;
            _cardsRectTransformList[i].localPosition = new Vector3(correctX, correctY, -correctZ);
            _cardsRectTransformList[i].rotation = Quaternion.Euler(correctRotation);
        }
    }
}
