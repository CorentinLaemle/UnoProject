using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HandManager))]
public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private CardBehaviour[] _cardsInHand;
    [SerializeField] private CardBehaviour[] _playableCards;
    [SerializeField] private float[] _playablePriorities;
    [SerializeField] private BrainSettings _brainSettings;

    private HandManager _myHandManager;
    private int _myPlayerIndex;
    private int _nextPlayer;
    private Card _activeCard;

    private int[] _colorPriorities;
    private int[] _valuesPriorities;
    private float[] _playerPriorities;
    private int[] _cardNumberPerColor;

    private void Awake()
    {
        _myHandManager = gameObject.GetComponent<HandManager>();
        _myPlayerIndex = _myHandManager.MyPlayerIndex;
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnTurnStart += StartTurnUpdating;
        CustomGameEvents.GetInstance().OnCardSelected += UpdatePlayerPriorities;
        CustomGameEvents.GetInstance().OnPlayerHasDrawn += ReactToPlayerDrawing;
        CustomGameEvents.GetInstance().OnActiveCardChanged += UpdateActiveCard;

        _playerPriorities = new float[GameManager.GetInstance()._players.Length];
        _colorPriorities = new int[_brainSettings.Colours];
        _valuesPriorities = new int[_brainSettings.MaxCardValue];
    }

    private void StartTurnUpdating(int playerIndex)
    {
        if(playerIndex == _myPlayerIndex)
        {
            _cardsInHand = new CardBehaviour[_myHandManager._cardsInHand.Count];
            for (int i = 0; i < _cardsInHand.Length; i++)
            {
                _cardsInHand[i] = _myHandManager._cardsInHand[i].GetComponent<CardBehaviour>();
            }
            _nextPlayer = GameManager.GetInstance().DetermineNextActivePlayer();
        }
        ThinkThinkThink();
    }

    private void ThinkThinkThink() 
    {
        int playableCards = 0;
        int cardCount = 0;

        _cardNumberPerColor = new int[_colorPriorities.Length];

        //for(int i = 0; i < _cardsInHand.Length; i++)
        //{
        //    int colorIndex = (int)_cardsInHand[i].MyCard._cardColor;
        //    _cardNumberPerColor[colorIndex]++;

        //    if (_cardsInHand[i]._isPlayable)
        //    {
        //        playableCards++;
        //    }
        //}

        foreach (CardBehaviour card in _cardsInHand) //we go through the cardsInHand array a first time, to know how many playable cards it contains
        {
            _cardNumberPerColor[(int)card.MyCard._cardColor]++;

            if (card._isPlayable)
            {
                playableCards++;
            }
        }
        _playableCards = new CardBehaviour[playableCards]; //We create an array of playable cards from which the AI will choose which one to pick
        foreach (CardBehaviour card in _cardsInHand) //the we populate the new array with our playable cards
        {
            if (card._isPlayable)
            {
                _playableCards[cardCount] = card;
                cardCount ++;
            }
        }

        //Then, we calculate the priority of each card from the _playableCards array, using the values from each corresponding parameters
        for(int i = 0; i < cardCount; i++)
        {
            int myValue = _playableCards[i].MyCard._cardValue;

            int colorPriority = _colorPriorities[(int)_playableCards[i].MyCard._cardColor] + _cardNumberPerColor[(int)_playableCards[i].MyCard._cardColor];
            float valuePriority = _valuesPriorities[myValue] * ((myValue > 11 && myValue != 13) ? _brainSettings.SkipCardFactor : 1);
            float playerPriority = (myValue > 11 && myValue != 13) ? _playerPriorities[_nextPlayer] : 0; //we only take playerPriority into account for skipping cards

            _playablePriorities[i] = colorPriority + valuePriority + playerPriority;
        }
        BrainBlast(cardCount);
    }

    private void BrainBlast(int cardsNumber)
    {
        int bestCardIndex = 0;
        float bestCardPriority = 0f;

        if(cardsNumber == 0)
        {
            _myHandManager.ClickAndDraw();
            StartTurnUpdating(_myPlayerIndex); //Since one may play the card they just drew
            return;
        }

        for(int i = 0; i < cardsNumber; i++)
        {
            if(_playablePriorities[i] > bestCardPriority)
            {
                bestCardIndex = i;
                bestCardPriority = _playablePriorities[i];
            }
        }

        _playableCards[bestCardIndex].ClickOnCard();

        //todo : terminer la prise de décision
        //todo : remplir dans unity les paramètres de brainsettings

        //si on joue un 13 ou 14, on parcourt _cardNumberPerColor pour savoir quelle couleur privilégier
    }

    #region ParametersUpdates
    private void UpdateActiveCard(Card card)
    {
        _activeCard = card;
    }

    private void UpdatePlayerPriorities(Card cardPlayed, int playerIndex)
    {
        int nextPlayer = GameManager.GetInstance().DetermineNextActivePlayer();

        if (nextPlayer == _myPlayerIndex && _brainSettings.IsVengeful) //Builds-up a thrist for vengeance against those who have wronged this player
        {
            if(cardPlayed._cardValue >= 11 && cardPlayed._cardValue != 14) //we don't care if the player played anything else than a skipping card
            {
                _playerPriorities[playerIndex] += _brainSettings.PlayerPriorityIncrement * _brainSettings.Vengefulness;
            }
            return;
        }

        if(playerIndex == _myPlayerIndex && _brainSettings.IsPityful) //Diminishes the will to actively hurt a specific player
        {
            if (cardPlayed._cardValue >= 11 && cardPlayed._cardValue != 14)
            {
                _playerPriorities[nextPlayer]-= _brainSettings.PlayerPriorityIncrement * _brainSettings.Pityfulness;
            }
            return;
        }
    }

    private void ReactToPlayerDrawing(int playerIndex) //this method is used whenever another player can't play
    {
        if (_brainSettings.IsExecutionner) //The AI will prioritize cards of the same color and value that couldn't be played on
        {
            foreach (int color in _colorPriorities)
            {
                if (color == (int)_activeCard._cardColor)
                {
                    _colorPriorities[color] += _brainSettings.ColorPriorityIncrement;
                }
            }
            foreach (int cardValue in _valuesPriorities)
            {
                if (cardValue == _activeCard._cardValue)
                {
                    _valuesPriorities[cardValue] += _brainSettings.ValuePriorityIncrement;
                }
            }
        }
    }
    #endregion

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnStart -= StartTurnUpdating;
        CustomGameEvents.GetInstance().OnCardSelected -= UpdatePlayerPriorities;
        CustomGameEvents.GetInstance().OnPlayerHasDrawn -= ReactToPlayerDrawing;
        CustomGameEvents.GetInstance().OnActiveCardChanged -= UpdateActiveCard;
    }
}
