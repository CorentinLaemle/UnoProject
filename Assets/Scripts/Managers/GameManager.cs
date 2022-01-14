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
    private float _turnBeginTimeMarker;
    private bool _isGamePaused;

    [SerializeField] private GameObject _wildChangePanel;

    private void Start()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded += PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnGameStart += StartGame;
        CustomGameEvents.GetInstance().OnCardPlayed += ProcessCardEffects;

        _wildChangePanel.SetActive(false);
    }

    private void Update()
    {
        if ((!_isGamePaused) && (_turnBeginTimeMarker + _turnTime == Time.time))
        {
            EndTurn();
        }
        if (_isGamePaused)
        {
            _turnBeginTimeMarker -= Time.deltaTime;
        }
    }

    //Gets called once at the start of the game, after the deck has finished shuffling
    private void PrepareFirstTurn()
    {
        _activePlayer = 0;
        _totalCardsToGive = _players.Length * _cardsToDistribute;
        _totalCardsGiven = 0;
        _isGamePaused = false;
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
        CustomGameEvents.GetInstance().TurnStart(_activePlayer);
    }

    //Is used by the end turn button
    public void CallEndTurn()
    {
        EndTurn();
    }

    private void EndTurn()
    {
        CustomGameEvents.GetInstance().TurnEnd();
        DetermineNextActivePlayer(_activePlayer, _turnType);
        CustomGameEvents.GetInstance().TurnStart(_activePlayer);
        _turnBeginTimeMarker = Time.time;
    }

    private void DetermineNextActivePlayer(int currentPlayer, TurnType turnType)
    {
        int nextPlayer = 0;
        switch (turnType)
        {
            case TurnType.clockwise:
                nextPlayer = currentPlayer + 1;
                if (currentPlayer == _players.Length - 1) { nextPlayer = 0; }
                break;
            case TurnType.counterClockwise:
                nextPlayer = currentPlayer - 1;
                if (currentPlayer == 0) { nextPlayer = _players.Length - 1; }
                break;
            default:
                Debug.Log(gameObject.name + " : DetermineNextActivePlayer error");
                break;
        }
        _arrows[_activePlayer].SetActive(false);
        _arrows[nextPlayer].SetActive(true);
        _activePlayer = nextPlayer;
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
                EndTurn();
                break;
            case 1:
                EndTurn();
                break;
            case 2:
                EndTurn();
                break;
            case 3:
                EndTurn();
                break;
            case 4:
                EndTurn();
                break;
            case 5:
                EndTurn();
                break;
            case 6:
                EndTurn();
                break;
            case 7:
                EndTurn();
                break;
            case 8:
                EndTurn();
                break;
            case 9:
                EndTurn();
                break;

            case 10 : //"invert turn"
                InvertTurnType();
                EndTurn();
                break;

            case 11: //"skip next" --> we call DetermineNextActivePlayer once here, then another time after the switch 
                DetermineNextActivePlayer(_activePlayer, _turnType);
                EndTurn();
                break;

            case 12: //"skip next" + "draw 2"
                DetermineNextActivePlayer(_activePlayer, _turnType);
                _players[_activePlayer].CallDrawCard(2);
                EndTurn();
                break;

            case 13: //"color change"
                ChooseActiveColor(); //--> will call TurnEnd() by itself so we don't use it here
                break;

            case 14: //"skip next" + "draw 4" + "color change"
                DetermineNextActivePlayer(_activePlayer, _turnType);
                _players[_activePlayer].CallDrawCard(4);
                ChooseActiveColor();
                break;

            default:
                Debug.Log("error : ProcessCardEffects cardValue incorrect");
                break;
        }
    }

    private void ChooseActiveColor()
    {
        _wildChangePanel.SetActive(true);
        _isGamePaused = true;
    }

    //This method will be called upon clicking on a color after ChooseActiveColor
    public void WildChange(Card card)
    {
        _isGamePaused = false;
        _wildChangePanel.SetActive(false);
        CustomGameEvents.GetInstance().ActiveCardChanged(card);
        EndTurn();
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

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnCardPlayed -= ProcessCardEffects;
        CustomGameEvents.GetInstance().OnFirstShuffleEnded -= PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnGameStart -= StartGame;
    }
}
