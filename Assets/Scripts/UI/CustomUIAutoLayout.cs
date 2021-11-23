using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUIAutoLayout : MonoBehaviour
{
    [SerializeField] private enum CustomLayoutType
    {
        top,
        left,
        bottom,
        right,
        discard
    }

    [SerializeField] private int _startCardsInterval;
    [SerializeField] private int _spaceDecrementStep;
    [SerializeField] private CustomLayoutType _myLayoutType;
    [SerializeField] private GameObject _cardPrefab;
    public bool _isLeftHandedMode;

    public List<RectTransform> _cardsRectTransformList;

    private RectTransform _rectTransform;
    private RectTransform _cardRectTransform;
    private int _cardsNumber;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cardRectTransform = _cardPrefab.GetComponent<RectTransform>();
        _cardsRectTransformList = new List<RectTransform>();
    }

    #region Reposition cards in hand
    //This method is called by the corresponding HandManager whenever it draws a card
    public void RepositionCardsInHand()
    {
        _cardsNumber = (TryGetComponent<HandManager>(out HandManager handManager))? handManager._cardsInHand.Count : 0;
        if(_cardsNumber == 0)
        {
            Debug.Log("HandManager script missing on" + gameObject.name);
            return;
        }

        if(_cardsNumber != _cardsRectTransformList.Count)
        {
            Debug.Log(gameObject.name + " : CandManager and CustomUILayout have inconsistent cards amount");
            return;
        }

        //Depending on the selected CustomLayoutType, we call the corresponding method with the following parameters :
        //Rotation of the cards, anchor point (min and max are the same), and the direction used to place the cards in hand (from the 1st to the last)
        switch (_myLayoutType)
        {
            case CustomLayoutType.bottom:
                RepositionCardsBottom(Vector3.zero, new Vector2(0, 0.5f), Vector3.right);
                break;
            case CustomLayoutType.left:
                RepositionCardsLeft(new Vector3(0, 0, -90), new Vector2(0.5f, 1), Vector3.down);
                break;
            case CustomLayoutType.top:
                RepositionCardsTop(new Vector3(0, 0, 180), new Vector2(1, 0.5f), Vector3.left);
                break;
            case CustomLayoutType.right:
                RepositionCardsRight(new Vector3(0, 0, 90), new Vector2(0.5f, 0), Vector3.up);
                break;
            case CustomLayoutType.discard:
                Debug.Log("Error : CustomLayoutType is discard but the hand method has been called.");
                    break;
            default:
                Debug.Log("CustomUIAutoLayout error : the selected CustomLayoutType doesn't exist.");
                break;
        }
    }

    //Since all four methods work the same way, commentary is written in details for the RepositionCardsBottom method below only
    private void RepositionCardsBottom(Vector3 cardRotation, Vector2 anchors, Vector2 nextCardIncrement)
    {
        //This line opens the possibility of changing the way cards appear in the hand. 
        //If _isLeftHandedMode is false (default), the last drawn card will be placed on the rigth of the hand.
        nextCardIncrement *= _isLeftHandedMode ? -1 : 1;

        //calculates the rought number of pixels we have at our disposal to place all the cards. We cast as an int since anchors may have float values.
        int handAreaLength = (int)(Screen.width * (_rectTransform.anchorMax.x - _rectTransform.anchorMin.x));

        int cardsInHand = _cardsNumber;
        //we only want to space out our cards if there is more than one card to space out
        int spaceBetweenCards = (cardsInHand > 1) ? _startCardsInterval : 0;
        //since we rotate each card to be facing the center of the screen, we'll always use the card width to determine the space taken by one card 
        int cardWidth = (int)_cardRectTransform.rect.width;

        //determines if we can place all the cards in hand using the current spaceBetweenCards parameter
        bool isEnoughSpace = handAreaLength > cardWidth * cardsInHand;

        if (!isEnoughSpace)
        {
            while (!isEnoughSpace)
            {
                //if not, recalculates by decrementing spaceBetweenCards
                spaceBetweenCards -= _spaceDecrementStep;
                isEnoughSpace = (handAreaLength > (cardWidth + spaceBetweenCards) * cardsInHand) ? true : false;
            }
        }

        //once we corrected the spaceBetween cards parameter so all cards in hand can visually fit inside of the hand area, we place them
        for(int i = 0; i < cardsInHand; i++)
        {
            int correctX = (handAreaLength / (cardsInHand + 1)) * (i + 1) - handAreaLength / 2;
            if(correctX == 0) //We add this line in order to avoid errors when recognizing cards in the hand when played. Should be invisible by eye
            {
                correctX = 1;
            }

            _cardsRectTransformList[i].anchorMin = anchors;
            _cardsRectTransformList[i].anchorMax = anchors;
            _cardsRectTransformList[i].localPosition = new Vector3(correctX, 0) * nextCardIncrement;
            _cardsRectTransformList[i].rotation = Quaternion.Euler(cardRotation);
        }
    }
    private void RepositionCardsLeft(Vector3 cardRotation, Vector2 anchors, Vector2 nextCardIncrement)
    {
        nextCardIncrement *= _isLeftHandedMode ? -1 : 1;

        int handAreaHeight = (int)(Screen.height * (_rectTransform.anchorMax.y - _rectTransform.anchorMin.y));

        int cardsInHand = _cardsNumber;
        int spaceBetweenCards = (cardsInHand > 1) ? _startCardsInterval : 0;
        int cardWidth = (int)_cardRectTransform.rect.width;

        bool isEnoughSpace = handAreaHeight > cardWidth * cardsInHand;

        if (!isEnoughSpace)
        {
            while (!isEnoughSpace)
            {
                spaceBetweenCards -= _spaceDecrementStep;
                isEnoughSpace = handAreaHeight > ((cardWidth + spaceBetweenCards) * cardsInHand);
            }
        }

        for (int i = 0; i < cardsInHand; i++)
        {
            int correctY = (handAreaHeight / (cardsInHand + 1)) * (i +1) - handAreaHeight /2;
            if (correctY == 0)
            {
                correctY = 1;
            }

            _cardsRectTransformList[i].anchorMin = anchors;
            _cardsRectTransformList[i].anchorMax = anchors;
            _cardsRectTransformList[i].localPosition = new Vector3(0, correctY) * nextCardIncrement;
            _cardsRectTransformList[i].rotation = Quaternion.Euler(cardRotation);
        }
    }
    private void RepositionCardsTop(Vector3 cardRotation, Vector2 anchors, Vector2 nextCardIncrement)
    {
        nextCardIncrement *= (_isLeftHandedMode) ? -1 : 1;

        int handAreaLength = (int)(Screen.width * (_rectTransform.anchorMax.x - _rectTransform.anchorMin.x));

        int cardsInHand = _cardsNumber;
        int spaceBetweenCards = (cardsInHand > 1) ? _startCardsInterval : 0;
        int cardWidth = (int)_cardRectTransform.rect.width;

        bool isEnoughSpace = handAreaLength > cardWidth * cardsInHand;
        if (!isEnoughSpace)
        {
            while (!isEnoughSpace)
            {
                spaceBetweenCards -= _spaceDecrementStep;
                isEnoughSpace = (handAreaLength > (cardWidth + spaceBetweenCards) * cardsInHand) ? true : false;
            }
        }

        for (int i = 0; i < cardsInHand; i++)
        {
            int correctX = (handAreaLength / (cardsInHand + 1)) * (i + 1) - handAreaLength / 2;
            if (correctX == 0)
            {
                correctX = 1;
            }

            _cardsRectTransformList[i].anchorMin = anchors;
            _cardsRectTransformList[i].anchorMax = anchors;
            _cardsRectTransformList[i].localPosition = new Vector3(correctX, 0) * nextCardIncrement;
            _cardsRectTransformList[i].rotation = Quaternion.Euler(cardRotation);
        }
    }
    private void RepositionCardsRight(Vector3 cardRotation, Vector2 anchors, Vector2 nextCardIncrement)
    {
        nextCardIncrement *= (_isLeftHandedMode) ? -1 : 1;

        int handAreaHeight = (int)(Screen.height * (_rectTransform.anchorMax.y - _rectTransform.anchorMin.y));

        int cardsInHand = _cardsNumber;
        int spaceBetweenCards = (cardsInHand > 1) ? _startCardsInterval : 0;
        int cardWidth = (int)_cardRectTransform.rect.width;

        bool isEnoughSpace = (handAreaHeight > cardWidth * cardsInHand);

        if (!isEnoughSpace)
        {
            while (!isEnoughSpace)
            {
                spaceBetweenCards -= _spaceDecrementStep;
                isEnoughSpace = (handAreaHeight > (cardWidth + spaceBetweenCards) * cardsInHand) ? true : false;
            }
        }

        for (int i = 0; i < cardsInHand; i++)
        {
            int correctY = (handAreaHeight / (cardsInHand + 1)) * (i + 1) - handAreaHeight / 2;
            if (correctY == 0)
            {
                correctY = 1;
            }

            _cardsRectTransformList[i].anchorMin = anchors;
            _cardsRectTransformList[i].anchorMax = anchors;
            _cardsRectTransformList[i].localPosition = new Vector3(0, correctY) * nextCardIncrement;
            _cardsRectTransformList[i].rotation = Quaternion.Euler(cardRotation);
        }
    }
#endregion
    public void RepositionCardsInDiscard(int playerIndex)
    {
        _cardsNumber = (TryGetComponent<DiscardManager>(out DiscardManager discardManager)) ? discardManager._discardList.Count : 0;
        if (_cardsNumber == 0)
        {
            Debug.Log("DiscardManager script missing on" + gameObject.name);
            return;
        }

        int lastPlayedCardIndex = _cardsRectTransformList.Count - 1;
        float rotationDelta = Random.Range(-15, 15);

        _cardsRectTransformList[lastPlayedCardIndex].localPosition = Vector3.zero;

        switch (playerIndex)
        {
            case 0: //deck or main player
                _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0,0,rotationDelta);
                break;
            case 1: //left
                _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta - 90);
                break;
            case 2: //top
                _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta - 180);
                break;
            case 3: //right
                _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta + 90);
                break;
        }
    }
}
