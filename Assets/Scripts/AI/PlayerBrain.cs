using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HandManager))]
public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private Card[] _cardsInHand;
    [SerializeField] private BrainSettings _brainSettings;

    private HandManager _myHandManager;
    private int _myPlayerIndex;
    private int _nextPlayer;
    private Card _activeCard;

    private int[] _colorPriorities;
    private int[] _valuesPriorities;
    private int[] _playerPriorities;

    private void Awake()
    {
        _myHandManager = GetComponent<HandManager>();
        _myPlayerIndex = _myHandManager.MyPlayerIndex;
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnTurnStart += StartTurnUpdate;
        CustomGameEvents.GetInstance().OnCardSelected += UpdatePlayerPriorities;
        CustomGameEvents.GetInstance().OnPlayerMustDraw += ReactToPlayerDrawing;
        CustomGameEvents.GetInstance().OnActiveCardChanged += UpdateActiveCard;

        _playerPriorities = new int[GameManager.GetInstance()._players.Length];
        _colorPriorities = new int[_brainSettings.Colours];
        _valuesPriorities = new int[_brainSettings.MaxCardValue];
    }

    //parameters : Card.CardColors , Card._cardValue, playerPriority , _GameManager._turnType;

    private void StartTurnUpdate(int playerIndex)
    {
        if(playerIndex == _myPlayerIndex)
        {
            _cardsInHand = new Card[_myHandManager._cardsInHand.Count];
            for (int i = 0; i < _cardsInHand.Length; i++)
            {
                _cardsInHand[i] = _myHandManager._cardsInHand[i].GetComponent<CardDisplay>()._card;
            }
            _nextPlayer = GameManager.GetInstance().DetermineNextActivePlayer();
        }
    }

    private void UpdateActiveCard(Card card)
    {
        _activeCard = card;
    }

    private void UpdatePlayerPriorities(Card cardPlayed, int playerIndex)
    {
        int nextPlayer = GameManager.GetInstance().DetermineNextActivePlayer();

        if (nextPlayer == _myPlayerIndex && _brainSettings.IsVengeful) //Builds-up a thrist for vengeance against those who have wronged this player
        {
            if(cardPlayed._cardValue >= 11)
            {
                _playerPriorities[playerIndex] += 1;
            }
            return;
        }

        if(playerIndex == _myPlayerIndex && _brainSettings.IsPityful) //Because why not
        {
            if (cardPlayed._cardValue >= 11)
            {
                _playerPriorities[nextPlayer] -= 1;
            }
            return;
        }
    }

    private void ReactToPlayerDrawing(int playerIndex)
    {
        //todo : continue project
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnStart -= StartTurnUpdate;
        CustomGameEvents.GetInstance().OnCardSelected -= UpdatePlayerPriorities;
        CustomGameEvents.GetInstance().OnPlayerMustDraw -= ReactToPlayerDrawing;
        CustomGameEvents.GetInstance().OnActiveCardChanged -= UpdateActiveCard;
    }
}
