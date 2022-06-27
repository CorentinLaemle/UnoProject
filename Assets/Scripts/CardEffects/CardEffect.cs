using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    //this script is the basis from which all cardEffects inherit

    [SerializeField] protected float _effectDuration;
    [SerializeField] protected float _maxDurationDelta;
    [SerializeField] protected bool _isEffectRunning;

    public float EffectDuration
    {
        get
        {
            return _effectDuration;
        }
        protected set
        {
            _effectDuration = value;
        }
    }
    public bool IsEffectRunning
    {
        get
        {
            return _isEffectRunning;
        }
    }

    public virtual void StartCardEffect(int cardValue) //this method is used to pre-calculate all the parameters for the effect
    {
        _isEffectRunning = true;
    }

    protected virtual IEnumerator PlayCardEffect() //this coroutine is the actual effect
    {
        yield return new WaitForSeconds(EffectDuration);
    }

    protected virtual void EndCardEffect() //this method tells (indirectly) the CardEffectMaster that the effect has finished playing
    {
        StopCoroutine(PlayCardEffect());
        _isEffectRunning = false;
    }
}
