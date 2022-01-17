using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable][CreateAssetMenu]
public class BrainSettings : ScriptableObject
{
    [SerializeField] [Tooltip("How many different colors there are in the game. Classic Uno : 5")] private int _numberOfCardColours;
    [SerializeField] [Tooltip("The max value of a card. Classic Uno : 15")] private int _maximumCardValue;

    [SerializeField] [Min(1)] [Tooltip("How much likely the AI will play a color that another player can't.")] private int _colorPriorityIncrement;
    [SerializeField] [Min(1)] [Tooltip("How much the AI will react to player-based factors.")] private int _playerPriorityIncrement;
    [SerializeField] [Min(1)] [Tooltip("How much likely the AI will play a value that another player can't.")] private int _valuePriorityIncrement;

    [SerializeField] [Min(1f)] [Tooltip("How much the AI will seek vengeance against those who skipped it in the pase.")] private float _vengefulness;
    [SerializeField] [Min(1f)] [Tooltip("How much the AI will take pity on a player it has skipped in the past.")] private float _pityfulness;
    [SerializeField] [Min(1f)] [Tooltip("How much likely the AI is to play a skip card if possible.")] private float _skipCardFactor;

    [SerializeField] private bool _isVengeful;
    [SerializeField] private bool _isPityful;
    [SerializeField] private bool _isExecutionner;

    public int Colours
    {
        get
        {
            return _numberOfCardColours;
        }
    }
    public int MaxCardValue
    {
        get
        {
            return _maximumCardValue;
        }
    }

    public int ColorPriorityIncrement
    {
        get
        {
            return _colorPriorityIncrement;
        }
    }
    public int PlayerPriorityIncrement
    {
        get
        {
            return _playerPriorityIncrement;
        }
    }
    public int ValuePriorityIncrement
    {
        get
        {
            return _valuePriorityIncrement;
        }
    }

    public float Vengefulness
    {
        get
        {
            return _vengefulness;
        }
    }
    public float Pityfulness
    {
        get
        {
            return _pityfulness;
        }
    }
    public float SkipCardFactor
    {
        get
        {
            return _skipCardFactor;
        }
    }

    public bool IsVengeful
    {
        get
        {
            return _isVengeful;
        }
    }
    public bool IsPityful
    {
        get
        {
            return _isPityful;
        }
    }
    public bool IsExecutionner
    {
        get
        {
            return _isExecutionner;
        }
    }
}
