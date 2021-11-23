using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGameEvents : MonoBehaviour
{
    private static CustomGameEvents _instance;
    public static CustomGameEvents GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
        }
        _instance = this;
    }
    //This script contains all the custom events used in the game. If you are coding anything else, you're doing it wrong !
    //The event names should all have the same structure : On+"Subject of the event"+"Action". The events are ordered alphabetically :

    //A
    public event Action<Card> OnActiveCardChanged;
    public void ActiveCardChanged(Card newActiveCard)
    {
        OnActiveCardChanged?.Invoke(newActiveCard);
    }
    //B
    //C
    public event Action<Card,int> OnCardPlayed;
    public void CardPlayed(Card playedCard, int playerIndex)
    {
        OnCardPlayed?.Invoke(playedCard,playerIndex);
    }
    public event Action<Card,int> OnCardSelected;
    public void CardSelected(Card selectedCard, int playerIndex)
    {
        OnCardSelected?.Invoke(selectedCard, playerIndex);
    }
    //D
    //E
    //F
    public event Action OnFirstShuffleEnded;
    public void FirstShuffleEnded()
    {
        OnFirstShuffleEnded?.Invoke();
    }
    //G
    public event Action OnGameStart;
    public void GameStart()
    {
        OnGameStart?.Invoke();
    }
    //H
    //I
    //J
    //K
    //L
    //M
    //N
    //O
    //P
    //Q
    //R
    //S
    //T
    public event Action<int> OnTurnBegin;
    public void TurnBegin(int playerindex)
    {
        OnTurnBegin?.Invoke(playerindex);
    }

    public event Action OnTurnEnd;
    public void TurnEnd()
    {
        OnTurnEnd?.Invoke();
    }
    //U
    //V
    //W
    //X
    //Y
    //Z
}
