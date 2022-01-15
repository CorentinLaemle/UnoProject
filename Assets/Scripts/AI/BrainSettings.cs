using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable][CreateAssetMenu]
public class BrainSettings : ScriptableObject
{
    [SerializeField] private int _numberOfCardColours;
    [SerializeField] private int _maximumCardValue;
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
