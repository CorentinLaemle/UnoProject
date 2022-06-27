using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoGameMaster : GameMaster
{
    protected enum AnimationEffects
    {
        invert,
        skipAndDraw,
        wild
    }

    [SerializeField] protected GameObject _wildChangePanel;
    [SerializeField] private GameObject _endGamePanel;

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
        CustomGameEvents.GetInstance().OnCardPlayedAndDiscarded += PlayCardEffectAnimation;
        CustomGameEvents.GetInstance().OnActiveCardChanged += SetCurrentActiveCard;

        _wildChangePanel.SetActive(false);
        _endGamePanel.SetActive(false);
    }

    protected override void PausePlaying() //this method is called by the GameMaster script in order to disable all card outlines in the player's hand
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
    }

    protected override void ResumePlaying() 
    {
        CustomGameEvents.GetInstance().ActiveCardChanged(ActiveCard);
    }


    protected override void EndTurn() //does all the end of turn actions
    {
        if (DetermineWinner())
        {
            _arrows[ActivePlayer].SetActive(false);
            PausePlaying();
            _endGamePanel.SetActive(true);
            return;
        }

        SetNextActivePlayer(ActivePlayer, CurrentTurnType);
        CustomGameEvents.GetInstance().ActiveCardChanged(ActiveCard);
        CustomGameEvents.GetInstance().TurnStart(ActivePlayer);
    }

    private void SetCurrentActiveCard(Card card)
    {
        ActiveCard = card;
    }

    private void SetNextActivePlayer(int currentPlayer, TurnType turnType) //this method is called by the GameMaster script whenever a skip card is player, or when the turn ends
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
    public int GetNextActivePlayer() //this method is called by the PlayerBrain (AI) to know which player is the next to play
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

    private void PlayCardEffectAnimation(Card cardPlayed, int playerIndex) //this method is called by a CustomGameEvent, whenever a card is played
    {
        int cardValue = cardPlayed._cardValue;
        if (cardValue >= 10 && cardValue < 15) //if the played card has a special effect, we start this effect through CardEffectsmaster
        {
            CardEffectsMaster.GetInstance().StartCardEffectAnim(cardValue);
            StartCoroutine(AnimStatusCheck(cardPlayed, playerIndex));
        }
        else //otherwise, we call ProcessCardEffects to process the actual effects of the card
        {
            ProcessCardEffects(cardPlayed, playerIndex);
        }
    }

    protected void ProcessCardEffects(Card cardPlayed, int playerIndex) //This method will be used to process the effects of every played card. It is called though a CustomGameEvent
    {
        StopCoroutine(AnimStatusCheck(cardPlayed, playerIndex));

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
                EndTurn();
                break;

            case 11: //"skip next" --> we call DetermineNextActivePlayer once here, then another time after the switch 
                SetNextActivePlayer(ActivePlayer, CurrentTurnType);
                EndTurn();
                break;

            case 12: //"skip next" + "draw 2"
                SetNextActivePlayer(ActivePlayer, CurrentTurnType);
                if (!_players[ActivePlayer].TryDrawCard(2))
                {
                    _players[ActivePlayer].TryDrawCard(2);
                }
                EndTurn();
                break;

            case 13: //"color change"
                ChooseActiveColor(playerIndex); //EndTurn() will be called AFTER the color has been chosen, directly in the WildChange() method, called by ChooseActiveColor()
                break;

            case 14: //"color change" + "skip next" + "draw 4"
                SetNextActivePlayer(ActivePlayer, CurrentTurnType);
                if (!_players[ActivePlayer].TryDrawCard(4))
                {
                    _players[ActivePlayer].TryDrawCard(4);
                }
                ChooseActiveColor(playerIndex);  //EndTurn() will be called AFTER the color has been chosen, directly in the WildChange() method, called by ChooseActiveColor()
                break;

            default:
                Debug.Log("error : ProcessCardEffects cardValue incorrect");
                break;
        }
    }

    private IEnumerator AnimStatusCheck(Card card, int playerIndex) //this coroutine will check regularly to know if the called cardEffect animation is done
    {
        while(!CardEffectsMaster.GetInstance().IsAnimOver)
        {
            yield return new WaitForSeconds(_animStatusCheckDelay);
        }
        Debug.Log("Anim over, processing card effect");
        ProcessCardEffects(card, playerIndex);
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

    public void WildChange(Card card) //This method will also be called upon clicking on a color after ChooseActiveColor
    {
        ActiveCard = card;
        ResumePlaying();
        _wildChangePanel.SetActive(false);
        EndTurn(); //todo : implanter l'appel de l'anim de choix de la couleur ; cette dernière appelera EndTurn()
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
        CustomGameEvents.GetInstance().OnCardPlayedAndDiscarded -= PlayCardEffectAnimation;
        CustomGameEvents.GetInstance().OnActiveCardChanged += SetCurrentActiveCard;
    }
}
