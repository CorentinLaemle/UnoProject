using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable][CreateAssetMenu]
public class BrainSettings : ScriptableObject
{
    [SerializeField] private int _numberOfCardColours;
    [SerializeField] private int _maximumCardValue;

    [SerializeField] [Min(1)] private int _colorPriorityIncrement;
    [SerializeField] [Min(1)] private int _playerPriorityIncrement;
    [SerializeField] [Min(1)] private int _valuePriorityIncrement;

    [SerializeField] [Min(0.1f)] private float _vengefulness;
    [SerializeField] [Min(0.1f)] private float _pityfulness;
    [SerializeField] [Min(0.1f)] private float _skipCardFactor;

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
