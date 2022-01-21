using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHandManager : HandManager
{ 
    //This script is a temporary script whose goal is to transform the current PlayerBrain : Monobehaviour into PlayerBrain : HandManager 

    //HandManager variables
    public List<GameObject> _cardsInHand;

    [SerializeField] [Tooltip("Bottom = 0, left = 1, top = 2 and right = 3")] private int _myPlayerIndex;
    [SerializeField] private bool _isMainPlayerHand;
    [SerializeField] private bool _isCurrentTurnActivePlayer;
    [SerializeField] private GameObject _cardPrefab;

    private bool _isForcedToDraw;
    private HandAutoLayout _handAutoLayout;
    private bool _hasDrawnThisTurn;

    //PlayerBrain variables
    [SerializeField] private CardBehaviour[] _cardBehaviours;
    [SerializeField] private CardBehaviour[] _playableCards;
    [SerializeField] private float[] _playablePriorities;
    [SerializeField] private BrainSettings _brainSettings;

    private HandManager _myHandManager;
    private Card _activeCard;
    //private int _myPlayerIndex;
    private int _nextPlayer;
    private bool _hasDrawn;
    private float _reactionTime;

    private float[] _playerPriorities;
    private int[] _colorPriorities;
    private int[] _valuesPriorities;
    private int[] _cardNumberPerColor;
}
