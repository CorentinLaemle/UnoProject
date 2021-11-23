using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;
    }

    public enum TurnType
    {
        clockwise,
        counterClockwise
    }

    public TurnType _turnType;
    [SerializeField] [Range(0,3)] private int _activePlayer;
    [SerializeField] private GameObject[] _arrows;
    [SerializeField] private HandManager[] _players;
    [SerializeField] private int _cardsToDistribute;
    private int _totalCardsGiven;
    private int _totalCardsToGive;
    [Range(0, 1)] public float _delayBetweenTryDraws;
    [Range(5,20)] public float _turnTime;

    private void Start()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded += PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnGameStart += StartGame;
        CustomGameEvents.GetInstance().OnCardPlayed += ProcessCardEffects;
        CustomGameEvents.GetInstance().OnTurnEnd += TurnEnded;
    }

    //Gets called once at the start of the game, after the deck has finished shuffling
    private void PrepareFirstTurn()
    {
        _activePlayer = 0;
        _totalCardsToGive = _players.Length * _cardsToDistribute;
        _totalCardsGiven = 0;
        StartCoroutine(DistributeCards());
    }

    private IEnumerator DistributeCards()
    {
        for(int i = 0; i < _totalCardsToGive; i++)
        {
            //Checks if the active player can draw a card. If possible, he will draw one ; if not, the deck will shuffle instead.
            bool isCardGiven = _players[_activePlayer].CallDrawCard(1);

            if(_activePlayer == _players.Length -1)
            {
                _activePlayer = -1;
            }
            _activePlayer++;
            _totalCardsGiven++;

            if (!isCardGiven)
            {
                if (_activePlayer == 0)
                {
                    _activePlayer = _players.Length - 1;
                }
                _activePlayer--;
                _totalCardsGiven--;
            }
            yield return new WaitForSeconds(_delayBetweenTryDraws);
        }

        if(_totalCardsGiven == _totalCardsToGive)
        {
            CustomGameEvents.GetInstance().GameStart();
        }
        else
        {
            Debug.Log("Error : for ended but _totalCardsGiven != _totalCardsToGive");
        }
    }

    //Gets called once at the start of the game, after all players have drawn their seven starting cards
    private void StartGame()
    {
        StopCoroutine(DistributeCards());
        _activePlayer = Random.Range(0, 4);
        _arrows[_activePlayer].SetActive(true);
        CustomGameEvents.GetInstance().TurnBegin(_activePlayer);
    }

    //This method will be used to process the effects of every played card.
    //It will be called after the card has been played, and before the player's turn ends.
    //todo : terminer la méthode avec les effets restants
    private void ProcessCardEffects(Card cardPlayed, int playerIndex)
    {
        switch (cardPlayed._cardValue)
        {
            //Number cards go from 0 to 9 --> we don't do anything special for those
            case 0:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 1:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 2:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 3:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 4:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 5:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 6:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 7:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 8:
                CustomGameEvents.GetInstance().TurnEnd();
                break;
            case 9:
                CustomGameEvents.GetInstance().TurnEnd();
                break;

            case 10 : //"invert turn"
                InvertTurnType();
                CustomGameEvents.GetInstance().TurnEnd();
                break;

            case 11: //"skip card" --> we call DetermineNextActivePlayer once here, then another time after the switch 
                DetermineNextActivePlayer(_activePlayer, _turnType);
                CustomGameEvents.GetInstance().TurnEnd();
                break;

            case 12:
                //"skip card" + "draw 2"
                DetermineNextActivePlayer(_activePlayer, _turnType);
                _players[_activePlayer].CallDrawCard(2);
                CustomGameEvents.GetInstance().TurnEnd();
                break;

            case 13:
                //"color change"
                ChooseActiveColor(13); //--> will call TurnEnd() by itself so we don't use it here
                break;

            case 14:
                //"skip card" + "draw 4" + "color change"
                DetermineNextActivePlayer(_activePlayer, _turnType);
                _players[_activePlayer].CallDrawCard(4);
                ChooseActiveColor(14);
                break;

            default:
                Debug.Log("error : ProcessCardEffects cardValue incorrect");
                break;
        }
    }

    //Gets called by the OnTurnEnd custom event
    private void TurnEnded()
    {
        DetermineNextActivePlayer(_activePlayer, _turnType);
        CustomGameEvents.GetInstance().TurnBegin(_activePlayer);
    }

    private void InvertTurnType()
    {
        if (_turnType == TurnType.clockwise)
        {
            _turnType = TurnType.counterClockwise;
            return;
        }
        _turnType = TurnType.clockwise;
    }

    private void ChooseActiveColor(int cardValue)
    {
        Debug.Log("ChooseActiveColor method called");
        //todo : la méthode fera apparaître 4 formes à l'écran (1 par couleur)
        //cliquer sur une forme créera une nouvelle carte 
        //OnActiveCardChanged et lui donnera en paramètre
        //la valeur de la carte (13/14) et la couleur choisie
        CustomGameEvents.GetInstance().TurnEnd();
    }

    private void DetermineNextActivePlayer(int currentPlayer, TurnType turnType)
    {
        int nextPlayer = 0;
        switch (turnType)
        {
            case TurnType.clockwise:
                nextPlayer = currentPlayer + 1;
                if(currentPlayer == _players.Length -1) { nextPlayer = 0; }
                break;
            case TurnType.counterClockwise:
                nextPlayer = currentPlayer - 1;
                if(currentPlayer == 0) { nextPlayer = _players.Length -1; }
                break;
            default:
                Debug.Log(gameObject.name + " : DetermineNextActivePlayer error");
                break;
        }
        _arrows[_activePlayer].SetActive(false);
        _arrows[nextPlayer].SetActive(true);
        _activePlayer = nextPlayer;
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnTurnEnd -= TurnEnded;
        CustomGameEvents.GetInstance().OnCardPlayed -= ProcessCardEffects;
        CustomGameEvents.GetInstance().OnFirstShuffleEnded -= PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnGameStart -= StartGame;
    }
}
