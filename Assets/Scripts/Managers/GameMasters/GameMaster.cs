using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public enum TurnType
    {
        clockwise,
        counterClockwise
    }

    public HandManager[] _players;
    [Range(0, 1)] public float _delayBetweenTryDraws;
    [Range(5, 20)] public float _turnTime;

    [SerializeField] protected GameObject[] _arrows;
    [SerializeField] private int _cardsToDistribute;
    [SerializeField] protected DeckList _colorChangeDeckList;
    [SerializeField] protected int _activePlayer;
    [SerializeField] protected TurnType _currentTurnType;
    [SerializeField] [Range(0,3f)] protected float _aIReactionTime;
    [SerializeField] [Range(0, 3f)] protected float _effectsAnimationDuration;
    [SerializeField] protected string _webGLOpenPageOnQuit;

    protected int _mainPlayerIndex;
    protected int _winnderIndex;
    protected int _totalCardsGiven;
    protected int _totalCardsToGive;
    protected float _turnBeginTimeMarker;
    protected bool _isGamePaused;
    protected Card _activeCard;
    protected Card _impossibleCard;

    public virtual Card ActiveCard
    {
        get
        {
            if (_activeCard != null && _isGamePaused == false)
            {
                return _activeCard;
            }

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
            return _impossibleCard;
        }
        protected set
        {
            _activeCard = value;
        }
    }

    public virtual int ActivePlayer
    {
        get
        {
            return _activePlayer;
        }
        protected set
        {
            _activePlayer = value;
        }
    }
    public  virtual TurnType CurrentTurnType
    {
        get
        {
            return _currentTurnType;
        }
        protected set
        {
            _currentTurnType = value;
        }
    }
    public float AIReactionTime
    {
        get
        {
            return _aIReactionTime;
        }
    }
    public virtual int MainPlayerIndex
    {
        get
        {
            return _mainPlayerIndex;
        }
        protected set
        {
            _mainPlayerIndex = value;
        }
    }

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded += PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnGameStart += StartGame;
        CustomGameEvents.GetInstance().OnPlayerHasSkipped += EndTurn;
        CustomGameEvents.GetInstance().OnShuffleStart += PausePlaying;
        CustomGameEvents.GetInstance().OnShuffleEnd += ResumePlaying;

        GetMainPlayerIndex();

#if UNITY_WEBGL
        if(_webGLOpenPageOnQuit == null)
        {
            _webGLOpenPageOnQuit = "about:blank";
        }
#endif
    }

    protected virtual void Update()
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

    protected void GetMainPlayerIndex()
    {
        foreach(HandManager player in _players)
        {
            if (player.IsMainPlayerHand)
            {
                MainPlayerIndex = player.MyPlayerIndex;
                return;
            }
        }
    }

    protected void PrepareFirstTurn() //Gets called once at the start of the game, after the deck has finished shuffling
    {
        ActivePlayer = 1;
        _totalCardsToGive = _players.Length * _cardsToDistribute;
        _totalCardsGiven = 0;
        _isGamePaused = true;
        StartCoroutine(DistributeCards());
    }

    protected IEnumerator DistributeCards()
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
            _isGamePaused = false;
            CustomGameEvents.GetInstance().DistributeCardsEnded();
        }
        else
        {
            Debug.Log("Error : for ended but _totalCardsGiven != _totalCardsToGive");
        }
    }

    protected void StartGame() //Gets called once at the start of the game, after all players have drawn their seven starting cards
    {
        StopCoroutine(DistributeCards());
        ActivePlayer = Random.Range(0, 4) ;
        _arrows[ActivePlayer].SetActive(true);
        
        CustomGameEvents.GetInstance().TurnStart(ActivePlayer);
    }

    protected virtual void PausePlaying()
    {
        _isGamePaused = true;
    }

    protected virtual void ResumePlaying()
    {
        _isGamePaused = false;
    }

    protected virtual void EndTurn()
    {
    }

    public virtual void CallEndTurn() //Is used by the "end turn" button
    {
        EndTurn();
    }

    public virtual void QuitGame()
    {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE) 
    Application.Quit();
#elif (UNITY_WEBGL)
    Application.OpenURL(_webGLOpenPageOnQuit);
#endif
    }

    protected virtual void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnGameStart -= StartGame;
        CustomGameEvents.GetInstance().OnFirstShuffleEnded -= PrepareFirstTurn;
        CustomGameEvents.GetInstance().OnPlayerHasSkipped -= EndTurn;
        CustomGameEvents.GetInstance().OnShuffleStart -= PausePlaying;
        CustomGameEvents.GetInstance().OnShuffleEnd -= ResumePlaying;
    }
}
