using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameHelper : MonoBehaviour //WIP : a set of tips to present the in-game buttons. Future uncertain
{
    [SerializeField] private GameObject[] _tips;
    [SerializeField] private GameObject _tipsPanel;
    
    private int _nextTipIndex;

    private void Start()
    {
        foreach (GameObject tip in _tips)
        {
            tip.SetActive(false);
        }
        _nextTipIndex = 0;
    }

    public void GetNextTip()
    {
        _tipsPanel.SetActive(true);

        foreach (GameObject tip in _tips)
        {
            tip.SetActive(false);
        }

        if (_nextTipIndex <= _tips.Length)
        {
            if(_nextTipIndex != _tips.Length)
            {
                _tips[_nextTipIndex].SetActive(true);
            }
            _nextTipIndex++;
        }

        if (_nextTipIndex > _tips.Length)
        {
            foreach (GameObject tip in _tips)
            {
                tip.SetActive(false);
            }

            _tipsPanel.SetActive(false);
            _nextTipIndex = 0;
        }
    }
}
