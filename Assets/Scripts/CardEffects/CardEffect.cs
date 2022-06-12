using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    [SerializeField] protected string _effectName;
    [SerializeField] protected float _effectDuration;
    [SerializeField] protected float _maxDurationDelta;

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
    public string Name
    {
        get
        {
            return _effectName;
        }
    }

    public virtual void StartCardEffect(int cardValue) //this method is used to pre-calculate all the parameters for the effect
    {}

    protected virtual IEnumerator PlayCardEffect() //this coroutine is the actual effect
    {
        yield return new WaitForSeconds(EffectDuration);
    }

    protected virtual void EndCardEffect() //this method tells the CardEffectMaster that the effect has finished playing
    {
        StopCoroutine(PlayCardEffect());
        CardEffectsMaster.GetInstance().CardEffectEnded();
    }
}
