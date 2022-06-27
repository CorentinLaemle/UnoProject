using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayerBrain : HandManager
{
    //This script regulates the AI. It inherits from HandManager

    [SerializeField] private BrainSettings _brainSettings;
    [SerializeField] private CardBehaviour[] _playableCards;
    [SerializeField] private float[] _playablePriorities;

    private Card _activeCard;
    private int _nextPlayer;
    private float _reactionTime;

    private float[] _playerPriorities;
    private int[] _colorPriorities;
    private int[] _valuesPriorities;
    private int[] _cardNumberPerColor;

    protected override void Start()
    {
        base.Start();
        CustomGameEvents.GetInstance().OnCardSelected += UpdatePlayerPriorities;
        CustomGameEvents.GetInstance().OnPlayerHasDrawn += ReactToPlayerDrawing;
        CustomGameEvents.GetInstance().OnActiveCardChanged += UpdateActiveCard;

        _playableCards = new CardBehaviour[0];
        _playablePriorities = new float[0];

        _playerPriorities = new float[UnoGameMaster.GetInstance()._players.Length];
        _colorPriorities = new int[_brainSettings.Colours];
        _valuesPriorities = new int[_brainSettings.MaxCardValue];
        _cardNumberPerColor = new int[0];

        _reactionTime = UnoGameMaster.GetInstance().AIReactionTime;
    }

    protected override void StartTurn()
    {
        _nextPlayer = UnoGameMaster.GetInstance().GetNextActivePlayer();
        Invoke(nameof(ThinkThinkThink), _reactionTime); //We call ThinkThinkThink with a delay to mimic a normal player's behavior of not playing within 10 frames
    }

    private void ThinkThinkThink() //this method will determine the best playable cards for this turn, and give each of them a priority value
    {
        int playableCards = 0;
        int cardCount = 0;

        _cardNumberPerColor = new int[_colorPriorities.Length];

        for (int i = 0; i < _cardsInHand.Count; i++) //we go through the cardsInHand array a first time, to know how many playable cards it contains
        {
            int colorIndex = (int)_cardsInHand[i]._myCard._cardColor;
            _cardNumberPerColor[colorIndex]++;

            if (_cardsInHand[i]._isPlayable)
            {
                playableCards++;
            }
        }
        if(playableCards != 0)
        {
            _playableCards = new CardBehaviour[playableCards]; //We create an array of playable cards from which the AI will choose which one to pick
            foreach (CardBehaviour card in _cardsInHand) //the we populate the new array with our playable cards
            {
                if (card._isPlayable)
                {
                    _playableCards[cardCount] = card;
                    cardCount++;
                }
            }

            //Then, we calculate the priority of each card from the _playableCards array, using the values from each corresponding parameters
            for (int i = 0; i < cardCount; i++)
            {
                int myValue = _playableCards[i]._myCard._cardValue;

                int colorPriority = _colorPriorities[(int)_playableCards[i]._myCard._cardColor] + _cardNumberPerColor[(int)_playableCards[i]._myCard._cardColor];
                float valuePriority = _valuesPriorities[myValue] * ((myValue > 11 && myValue != 13) ? _brainSettings.SkipCardFactor : 1);
                float playerPriority = (myValue > 11 && myValue != 13) ? _playerPriorities[_nextPlayer] : 0; //we only take playerPriority into account for skipping cards

                _playablePriorities = new float[_playableCards.Length];
                _playablePriorities[i] = colorPriority + valuePriority + playerPriority;
            }
        }
        BrainBlast(cardCount);
    }

    private void BrainBlast(int cardsNumber) //this method will first determine the AI's actions depending on Thinkthinkthink's outcome
    {
        int bestCardIndex = 0;
        float bestCardPriority = 0f;

        if(cardsNumber == 0)
        {
            if (!HasDrawnThisTurn)
            {
                ClickAndDraw();
                return;
            }
            CustomGameEvents.GetInstance().PlayerHasSkipped();
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
        HasDrawnThisTurn = false;
        HasPlayedThisTurn = true;
        _playableCards[bestCardIndex].ClickOnCard();
    }

    #region ParametersUpdates
    public int MostInterestingColor() //is called by UnoGameMaster whenever this AI plays a wild (black) card
    {
        int bestColorIndex = 0;
        int mostCardInAColor = 0;

        for(int i  = 0; i < _cardNumberPerColor.Length; i++)
        {
            if(_cardNumberPerColor[i] > mostCardInAColor)
            {
                bestColorIndex = i;
                mostCardInAColor = _cardNumberPerColor[i];
            }
        }
        return bestColorIndex;
    }

    private void UpdateActiveCard(Card card)
    {
        _activeCard = card;
    }

    private void UpdatePlayerPriorities(Card cardPlayed, int playerIndex)
    {
        int nextPlayer = UnoGameMaster.GetInstance().GetNextActivePlayer();

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
        CustomGameEvents.GetInstance().OnCardSelected -= UpdatePlayerPriorities;
        CustomGameEvents.GetInstance().OnPlayerHasDrawn -= ReactToPlayerDrawing;
        CustomGameEvents.GetInstance().OnActiveCardChanged -= UpdateActiveCard;
    }
}
