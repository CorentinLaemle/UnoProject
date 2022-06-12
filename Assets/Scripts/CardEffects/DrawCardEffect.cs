using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardEffect : CardEffect
{
    [SerializeField] private Transform _impactsParent;
    [SerializeField] private Transform _repairsParent;
    [SerializeField] private GameObject _drawParent;

    private GameObject[] _impacts;
    private GameObject[] _repairs;

    private List<int> _indexesPile;
    private List<int> _selectedImpacts;

    private void Start()
    {
        int effects = (_impactsParent.childCount >= _repairsParent.childCount) ? _repairsParent.childCount : _impactsParent.childCount;
        
        _impacts = new GameObject[effects];
        _repairs = new GameObject[effects];
        for(int i = 0; i < effects; i++)
        {
            _impacts[i] = _impactsParent.GetChild(i).gameObject;
            _repairs[i] = _repairsParent.GetChild(i).gameObject;
            _indexesPile.Add(i);
        }
    }

    public override void StartCardEffect(int cardValue)
    {
        if(cardValue != 12 && cardValue != 14)
        {
            CardEffectsMaster.GetInstance().CardEffectEnded();
        }

        int cardsToDraw = 2;
        if(cardValue == 14)
        {
            cardsToDraw = 4;
        }

        List<int> indexPileCopy = _indexesPile;
        for(int i = 0; i < cardsToDraw; i++)
        {
            int j = indexPileCopy[Random.Range(0, indexPileCopy.Count)];
            _selectedImpacts.Add(j);
            indexPileCopy.RemoveAt(j);
        }
        
        _drawParent.SetActive(true);
        StartCoroutine(PlayCardEffect());
    }

    protected override IEnumerator PlayCardEffect()
    {
        EffectDuration += Random.Range(-1f, 1f) * _maxDurationDelta;
        yield return new WaitForSeconds(EffectDuration);

        int i = _selectedImpacts[0];
        _impacts[i].SetActive(true);
        _repairs[i].SetActive(false);
        //joue le son

        if (_selectedImpacts.Count == 0)
        {
            EndCardEffect();
        }
    }

    protected override void EndCardEffect()
    {
        for(int i = 0; i < _impacts.Length; i++)
        {
            _impacts[i].SetActive(false);
            _repairs[i].SetActive(true);
            _drawParent.SetActive(false);
        }

        base.EndCardEffect();
    }
}
