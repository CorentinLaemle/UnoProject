using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectsMaster : MonoBehaviour
{
    private static CardEffectsMaster _instance;
    public static CardEffectsMaster GetInstance()
    {
        return _instance;
    }

    [SerializeField] private float _testTime;
    [Tooltip("Card effects must be named respectively : Reverse, Skip, Draw, Wild or WildDraw. Other names will result in an error.")]
    [SerializeField] private CardEffect[] _cardEffects;

    private bool _isAnimOver;

    public bool IsAnimOver
    {
        get
        {
            return _isAnimOver;
        }
        set
        {
            _isAnimOver = value;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;

        IsAnimOver = false;
    }

    public void StartCardEffectAnim(int cardValue)
    {
        if(IsAnimOver == false)
        {
            Debug.Log("Error : tried to start a card effect while one was already playing");
            return;
        }

        IsAnimOver = false;
        switch (cardValue)
        {
            case 10://invert turn
                break;

            case 11://skip
                break;

            case 12://draw 2
                break;

            case 13://wild
                break;

            case 14://wild draw 4
                break;
        }
        Invoke(nameof(CardEffectEnded), _testTime);
    }

    public void CardEffectEnded()
    {
        IsAnimOver = true; //todo : finir d'implémenter
    }

    private void CallInvertTurn()
    {

    }
    private void CallSkip()
    {

    }
    private void CallDraw()
    {

    }
    private void CallWild()
    {

    }
    private void CallWildDraw()
    {

    }
}
