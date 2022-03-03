using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoGameMaster : GameMaster
{
    [SerializeField] protected GameObject _wildChangePanel;

    private static UnoGameMaster _instance;

    public override Card ActiveCard => base.ActiveCard;
    public override int ActivePlayer => base.ActivePlayer;
    public override TurnType CurrentTurnType => base.CurrentTurnType;
    public override int MainPlayerIndex { get => base.MainPlayerIndex; protected set => base.MainPlayerIndex = value; }

    public static UnoGameMaster GetInstance()
    {
        return _instance;
    }

    protected override void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;
    }

    protected override void Start()
    {
        base.Start();
        CustomGameEvents.GetInstance().OnCardPlayedAndDiscarded += ProcessCardEffects;
        CustomGameEvents.GetInstance().OnActiveCardChanged += SetCurrentActiveCard;

        _wildChangePanel.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void PausePlaying()
    {
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
        _isGamePaused = true;
    }

    protected override void ResumePlaying()
    {
        _isGamePaused = false;
        CustomGameEvents.GetInstance().ActiveCardChanged(ActiveCard);
    }


    protected override void EndTurn() //todo : rajouter la victoire/défaite
    {
        if (DetermineWinner())
        {
            Debug.Log("Player " + _winnderIndex + " has won!");
            _arrows[ActivePlayer].SetActive(false);
            PausePlaying();
            return;
        }

        GetNextActivePlayer(ActivePlayer, CurrentTurnType);
        CustomGameEvents.GetInstance().ActiveCardChanged(ActiveCard);
        CustomGameEvents.GetInstance().TurnStart(ActivePlayer);
        _turnBeginTimeMarker = Time.time;
    }

    private void SetCurrentActiveCard(Card card)
    {
        ActiveCard = card;
    }

    private void GetNextActivePlayer(int currentPlayer, TurnType turnType)
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
    public int GetNextActivePlayer()
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

    protected void ProcessCardEffects(Card cardPlayed, int playerIndex) //This method will be used to process the effects of every played card. It is called though a CustomGameEvent
    {
        ActiveCard = cardPlayed;

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

            case 10: //"invert turn"
                InvertTurnType();
                Invoke(nameof(EndTurn), _effectsAnimationDuration);
                break;

            case 11: //"skip next" --> we call DetermineNextActivePlayer once here, then another time after the switch 
                GetNextActivePlayer(ActivePlayer, CurrentTurnType);
                Invoke(nameof(EndTurn), _effectsAnimationDuration);
                break;

            case 12: //"skip next" + "draw 2"
                GetNextActivePlayer(ActivePlayer, CurrentTurnType);
                if (!_players[ActivePlayer].TryDrawCard(2))
                {
                    _players[ActivePlayer].TryDrawCard(2);
                }
                Invoke(nameof(EndTurn), _effectsAnimationDuration);
                break;

            case 13: //"color change"
                ChooseActiveColor(playerIndex); //EndTurn() will be called AFTER the color has been chosen, directly in the WildChange() method, called by ChooseActiveColor()
                break;

            case 14: //"color change" + "skip next" + "draw 4"
                ChooseActiveColor(playerIndex);  //EndTurn() will be called AFTER the color has been chosen, directly in the WildChange() method, called by ChooseActiveColor()

                GetNextActivePlayer(ActivePlayer, CurrentTurnType);
                if (!_players[ActivePlayer].TryDrawCard(4))
                {
                    _players[ActivePlayer].TryDrawCard(4);
                }
                break;

            default:
                Debug.Log("error : ProcessCardEffects cardValue incorrect");
                break;
        }
    }

    private void ChooseActiveColor(int playerIndex)
    {
        PausePlaying();

        if (playerIndex == MainPlayerIndex)
        {
            _wildChangePanel.SetActive(true);
            return;
        }

        bool isAI = _players[playerIndex].TryGetComponent<PlayerBrain>(out PlayerBrain playerBrain);
        int chosenColor = 0;
        if (isAI)
        {
            chosenColor = playerBrain.MostInterestingColor();
        }
        for(int i = 0; i < _colorChangeDeckList.cards.Count; i++)
        {
            if ((int)_colorChangeDeckList.cards[i]._cardColor == chosenColor)
            {
                WildChange(_colorChangeDeckList.cards[i]);
                break;
            }
        }
    }

    public void WildChange(Card card) //This method will be called upon clicking on a color after ChooseActiveColor
    {
        ActiveCard = card;
        ResumePlaying();
        _wildChangePanel.SetActive(false);
        Invoke(nameof(EndTurn), _effectsAnimationDuration);
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

    private bool DetermineWinner()
    {
        for(int i = 0; i < _players.Length; i++)
        {
            if(_players[i]._cardsInHand.Count == 0)
            {
                _winnderIndex = i;
                return true;
            }
        }
        return false;
    }

    public override void CallEndTurn()
    {
        if(_activePlayer == _mainPlayerIndex)
        {
            base.CallEndTurn();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CustomGameEvents.GetInstance().OnCardPlayed -= ProcessCardEffects;
        CustomGameEvents.GetInstance().OnActiveCardChanged += SetCurrentActiveCard;
    }
}
