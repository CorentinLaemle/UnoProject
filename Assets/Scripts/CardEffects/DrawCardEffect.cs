using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCardEffect : CardEffect
{
    [SerializeField] private Transform _impactsParent;
    [SerializeField] private Transform _repairsParent;
    [SerializeField] private GameObject _drawPoster;
    [SerializeField] private AudioSource _posterAS;
    [SerializeField] private AudioSource _bulletAS;

    private GameObject[] _impacts;
    private GameObject[] _repairs;

    private List<int> _indexesPile;
    private List<int> _selectedImpacts;

    private bool _bulletsShot;

    private void Start()
    {
        AudioManager.GetInstance().SetAudioSource(_posterAS, "WantedPoster");
        AudioManager.GetInstance().SetAudioSource(_bulletAS, "BulletShot");

        int effects = (_impactsParent.childCount >= _repairsParent.childCount) ? _repairsParent.childCount : _impactsParent.childCount;
        
        _impacts = new GameObject[effects];
        _repairs = new GameObject[effects];
        _indexesPile = new List<int>();
        _selectedImpacts = new List<int>();

        for (int i = 0; i < effects; i++)
        {
            _impacts[i] = _impactsParent.GetChild(i).gameObject;
            _repairs[i] = _repairsParent.GetChild(i).gameObject;
            _repairs[i].SetActive(true);
            _indexesPile.Add(i);
        }
    }

    public override void StartCardEffect(int cardValue) //determines which bullet impacts will be used for this effect, and calls on PlayCardEffect()
    {
        base.StartCardEffect(cardValue);

        if(cardValue != 12 && cardValue != 14)
        {
            EndCardEffect();
        }

        int cardsToDraw = 2;
        if(cardValue == 14)
        {
            cardsToDraw = 4;
        }
        _bulletsShot = false;

        List<int> indexPileCopy = new List<int>(_indexesPile);
        for(int i = 0; i < cardsToDraw; i++)
        {
            int index = Random.Range(0, indexPileCopy.Count);
            int value = indexPileCopy[index];
            _selectedImpacts.Add(value);
            indexPileCopy.RemoveAt(index);
        }
        StartCoroutine(PlayCardEffect());
    }

    protected override IEnumerator PlayCardEffect() //this coroutine plays the actual effect
    {
        _drawPoster.SetActive(true);

        while (_bulletsShot == false)
        {
            _bulletsShot = true;
            AudioManager.GetInstance().PlayPitchedSound(_posterAS);
            yield return new WaitForSeconds(EffectDuration);
        }

        while(_selectedImpacts.Count > 0)
        {
            int i = _selectedImpacts[0];
            _impacts[i].SetActive(true);
            _repairs[i].SetActive(false);
            _selectedImpacts.RemoveAt(0);
            AudioManager.GetInstance().PlayPitchedSound(_bulletAS);

            float duration = EffectDuration + Random.Range(-1f, 1f) * _maxDurationDelta;
            yield return new WaitForSeconds(duration);
        }
            EndCardEffect();
    }

    protected override void EndCardEffect() //this method resets the isActive bool of all gameObjects to their starting value
    {
        _drawPoster.SetActive(false);
        for (int i = 0; i < _impacts.Length; i++)
        {
            _impacts[i].SetActive(false);
            _repairs[i].SetActive(true);
        }
        base.EndCardEffect();
    }
}
