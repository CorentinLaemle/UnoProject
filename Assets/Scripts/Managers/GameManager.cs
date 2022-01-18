using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum TurnType
    {
        clockwise,
        counterClockwise
    }

    public HandManager[] _players;
    [Range(0, 1)] public float _delayBetweenTryDraws;
    [Range(5, 20)] public float _turnTime;

    [SerializeField] private GameObject[] _arrows;
    [SerializeField] private int _cardsToDistribute;
    [SerializeField] private GameObject _wildChangePanel;
    [SerializeField] private DeckList _colorChangeDeckList;
    [SerializeField] private int _activePlayer;
    [SerializeField] private TurnType _currentTurnType;

    private int _mainPlayerIndex;
    private int _totalCardsGiven;
    private int _totalCardsToGive;
    private float _turnBeginTimeMarker;
    private bool _isGamePaused;
    private Card _activeCard;
    private Card _impossibleCard;
    private static GameManager _instance;

    public int ActivePlayer
    {
        get
        {
            return _activePlayer;
        }
        private set
        {
            _activePlayer = value;
        }
    }
    public TurnType CurrentTurnType
    {
        get
        {
            return _currentTurnType;
        }
        private set
        {
            _currentTurnType = value;
        }
    }
    public Card ActiveCard
    {
        get
        {
            if(_activeCard != null)
            {
                return _activeCard;
            }

            if(_impossibleCard == null)
            {
                _impossibleCard = ScriptableObject.CreateInstance<Card>();
                for (int i = 0; i < _colorChangeDeckList.cards.Count; i++)
                {
                    if (_colorChangeDeckList.cards[i]._cardColor == Card.CardColor.invalid)
                    {
                        _impossibleCard = _colorChangeDeckList.cards[i];
                        break;
                    }
                }
            }
            return _impossibleCard;
        }
    }

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

    private void Start()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded += PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnGameStart += StartGame;
        CustomGameEvents.GetInstance().OnCardPlayed += ProcessCardEffects;
        CustomGameEvents.GetInstance().OnPlayerHasSkipped += EndTurn;
        CustomGameEvents.GetInstance().OnActiveCardChanged += GetCurrentActiveCard;
        CustomGameEvents.GetInstance().OnShuffleStart += PausePlaying;
        CustomGameEvents.GetInstance().OnShuffleEnd += ResumePlaying;

        _wildChangePanel.SetActive(false);
        GetMainPlayerIndex();
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

    private void GetMainPlayerIndex()
    {
        foreach(HandManager player in _players)
        {
            if (player.IsMainPlayerHand)
            {
                _mainPlayerIndex = player.MyPlayerIndex;
                return;
            }
        }
    }

    private void PrepareFirstTurn() //Gets called once at the start of the game, after the deck has finished shuffling
    {
        ActivePlayer = 0;
        _totalCardsToGive = _players.Length * _cardsToDistribute;
        _totalCardsGiven = 0;
        _isGamePaused = false;
        StartCoroutine(DistributeCards());
    }

    private IEnumerator DistributeCards()
    {
        for(int i = 0; i < _totalCardsToGive; i++)
        {
            //Checks if the active player can draw a card. If possible, it will draw one ; if not, the deck will shuffle instead, and we will retry drawing after the yield return instruction
            bool isCardGiven = _players[ActivePlayer].TryDrawCard(1);

            if(ActivePlayer == _players.Length -1)
            {
                ActivePlayer = -1;
            }
            ActivePlayer++;
            _totalCardsGiven++;

            if (!isCardGiven) //if the deck was shuffled instead of drawing a card, theses instructions make sure that we still draw the card we should have drawn
            {
                if (ActivePlayer == 0)
                {
                    ActivePlayer = _players.Length - 1;
                }
                ActivePlayer--;
                _totalCardsGiven--;
            }
            yield return new WaitForSeconds(_delayBetweenTryDraws);
        }

        if(_totalCardsGiven == _totalCardsToGive)
        {
            CustomGameEvents.GetInstance().DistributeCardsEnded();
        }
        else
        {
            Debug.Log("Error : for ended but _totalCardsGiven != _totalCardsToGive");
        }
    }

    private void StartGame() //Gets called once at the start of the game, after all players have drawn their seven starting cards
    {
        StopCoroutine(DistributeCards());
        ActivePlayer = Random.Range(0, 4) ;
        _arrows[ActivePlayer].SetActive(true);
        CustomGameEvents.GetInstance().TurnStart(ActivePlayer);
    }
     
    private void GetCurrentActiveCard(Card card)
    {
        if (!_isGamePaused)
        {
            _activeCard = card;
        }
    }

    private void PausePlaying()
    {
        _isGamePaused = true;

        if (_impossibleCard == null)
        {
            _impossibleCard = ScriptableObject.CreateInstance<Card>();
            for (int i = 0; i < _colorChangeDeckList.cards.Count; i++)
            {
                if (_colorChangeDeckList.cards[i]._cardColor == Card.CardColor.invalid)
                {
                    _impossibleCard = _colorChangeDeckList.cards[i];
                    break;
                }
            }
        }
        CustomGameEvents.GetInstance().ActiveCardChanged(_impossibleCard);
    }

    private void ResumePlaying()
    {
        _isGamePaused = false;
        CustomGameEvents.GetInstance().ActiveCardChanged(_activeCard);
    }
    private void ResumePlaying(Card card)
    {
        _isGamePaused = false;
        CustomGameEvents.GetInstance().ActiveCardChanged(card);
    }

    private void EndTurn()
    {
        CustomGameEvents.GetInstance().TurnEnd();
        DetermineNextActivePlayer(ActivePlayer, CurrentTurnType);
        CustomGameEvents.GetInstance().TurnStart(ActivePlayer);
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
        _arrows[ActivePlayer].SetActive(false);
        _arrows[nextPlayer].SetActive(true);
        ActivePlayer = nextPlayer;
    }
    public int DetermineNextActivePlayer()
    {
        int nextPlayer = 0;
        switch (CurrentTurnType)
        {
            case TurnType.clockwise:
                nextPlayer = ActivePlayer + 1;
                if (ActivePlayer == _players.Length - 1) { nextPlayer = 0; }
                break;
            case TurnType.counterClockwise:
                nextPlayer = ActivePlayer - 1;
                if (ActivePlayer == 0) { nextPlayer = _players.Length - 1; }
                break;
            default:
                Debug.Log(gameObject.name + " : DetermineNextActivePlayer error");
                break;
        }
        return nextPlayer;
    }

    private void ProcessCardEffects(Card cardPlayed, int playerIndex) //This method will be used to process the effects of every played card. It is called though a CustomGameEvent
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
                DetermineNextActivePlayer(ActivePlayer, CurrentTurnType);
                EndTurn();
                break;

            case 12: //"skip next" + "draw 2"
                DetermineNextActivePlayer(ActivePlayer, CurrentTurnType);
                _players[ActivePlayer].TryDrawCard(2);
                EndTurn();
                break;

            case 13: //"color change"
                ChooseActiveColor(playerIndex); //--> will call TurnEnd() by itself so we don't use it here
                break;

            case 14: //"skip next" + "draw 4" + "color change"
                DetermineNextActivePlayer(ActivePlayer, CurrentTurnType);
                _players[ActivePlayer].TryDrawCard(4);
                ChooseActiveColor(playerIndex);
                break;

            default:
                Debug.Log("error : ProcessCardEffects cardValue incorrect");
                break;
        }
    }

    private void ChooseActiveColor(int playerIndex)
    {
        if(playerIndex == _mainPlayerIndex)
        {
            _wildChangePanel.SetActive(true);
            PausePlaying();
            return;
        }
        int chosenColor = _players[playerIndex].GetComponent<PlayerBrain>().MostInterestingColor();
        foreach(Card card in _colorChangeDeckList.cards)
        {
            if((int)card._cardColor == chosenColor)
            {
                WildChange(card);
                return;
            }
        }
    }

    public void WildChange(Card card) //This method will be called upon clicking on a color after ChooseActiveColor
    {
        ResumePlaying(card);
        _wildChangePanel.SetActive(false);
        EndTurn();
    }

    private void InvertTurnType()
    {
        if (CurrentTurnType == TurnType.clockwise)
        {
            CurrentTurnType = TurnType.counterClockwise;
            return;
        }
        CurrentTurnType = TurnType.clockwise;
    }

    public void CallEndTurn() //Is used by the "end turn" button
    {
        EndTurn();
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnGameStart -= StartGame;
        CustomGameEvents.GetInstance().OnFirstShuffleEnded -= PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnCardPlayed -= ProcessCardEffects;
        CustomGameEvents.GetInstance().OnPlayerHasSkipped -= EndTurn;
        CustomGameEvents.GetInstance().OnActiveCardChanged += GetCurrentActiveCard;
        CustomGameEvents.GetInstance().OnShuffleStart -= PausePlaying;
        CustomGameEvents.GetInstance().OnShuffleEnd -= ResumePlaying;
    }
}
