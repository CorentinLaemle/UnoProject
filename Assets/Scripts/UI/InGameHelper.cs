using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHelper : MonoBehaviour
{
    [SerializeField] private GameObject[] _tips;
    [SerializeField] private GameObject _tipsPanel;
    
    private int _nextTipIndex;

    private void Start()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded += GetNextTip;

        foreach (GameObject tip in _tips)
        {
            tip.SetActive(false);
        }
        _nextTipIndex = 0;
    }

    public void GetNextTip()
    {
        if(_tipsPanel.activeInHierarchy == false)
        {
            _tipsPanel.SetActive(true);
            _nextTipIndex = 0;
        }

        foreach (GameObject tip in _tips)
        {
            tip.SetActive(false);
        }

        if (_nextTipIndex < _tips.Length)
        {
            _tips[_nextTipIndex].SetActive(true);
            _nextTipIndex++;
            return;
        }
        _tipsPanel.SetActive(false);
        _nextTipIndex = 0;
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded -= GetNextTip;
    }
}
