using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectsMaster : MonoBehaviour
{
    //this script is called by the UnoGameMaster whenever it needs to play a cardEffect
    //the specific cardEffect to play is chosen depending on the card value passed by the UnoGameMaster
    //this scripts will then monitor the progress of the cardEffect and change _isAnimOver's value (listened by UnoGameMaster) when the effect is over

    private static CardEffectsMaster _instance;
    public static CardEffectsMaster GetInstance()
    {
        return _instance;
    }

    [SerializeField] private float _testTime;
    [SerializeField] private GameObject _DrawPrefab;
    [SerializeField] private GameObject _ReversePrefab;
    [SerializeField] private GameObject _SkipPrefab;
    [SerializeField] private GameObject _WildPrefab;

    private DrawCardEffect _drawCardEffect;
    private CardEffect _reverseEffect;
    private CardEffect _skip;
    private CardEffect _wild;

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

        IsAnimOver = true;
    }

    private void Start()
    {
        if (_DrawPrefab.TryGetComponent(out DrawCardEffect draw))
        {
            _drawCardEffect = draw;
        }
        if (_ReversePrefab.TryGetComponent<CardEffect>(out CardEffect reverse))
        {
            _reverseEffect = reverse;
        }
        if (_SkipPrefab.TryGetComponent<CardEffect>(out CardEffect skip))
        {
            _skip = skip;
        }
        if (_WildPrefab.TryGetComponent<CardEffect>(out CardEffect wild))
        {
            _wild = wild;
        }
    }

    public void StartCardEffectAnim(int cardValue) //todo : finir d'implémenter les animations
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
                _isAnimOver = true;
                break;

            case 11://skip
                _isAnimOver = true;
                break;

            case 12://draw 2
                _drawCardEffect.StartCardEffect(cardValue);
                StartCoroutine(AnimOverChecker(_drawCardEffect));
                break;

            case 13://wild
                _isAnimOver = true; 
                break;

            case 14://wild draw 4
                _drawCardEffect.StartCardEffect(cardValue);
                StartCoroutine(AnimOverChecker(_drawCardEffect));
                break;
        }
    }

    private IEnumerator AnimOverChecker(CardEffect CE)
    {
        while(CE.IsEffectRunning)
        {
            yield return new WaitForSeconds(_testTime);
        }
        CardEffectEnded(CE);
    }

    public void CardEffectEnded(CardEffect CE)
    {
        StopCoroutine(AnimOverChecker(CE));
        IsAnimOver = true; //todo : finir d'implémenter
    }
}
