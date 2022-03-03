using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAudioCaller : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private string _soundName;
    [SerializeField] private bool _isPlayOnStart;

    void Start()
    {
        AudioManager.GetInstance().SetAudioSource(_source, _soundName);

        if (_isPlayOnStart)
        {
            PlaySound();
        }

    }
    
    public void PlaySound()
    {
        AudioManager.GetInstance().PlayPitchedSound(_source);
    }
}
