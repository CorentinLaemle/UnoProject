using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0f, 1f)] public float _spatialBlendValue;
    public FloatVariable _sfxVolumeMultiplier;
    public FloatVariable _musicVolumeMultiplier;
    
    public SoundData[] _sounds;
    
    [HideInInspector]
    public AudioSource _audioManagerAudioSource;
    
    private static AudioManager _instance;

    public static AudioManager GetInstance()
    {
        return _instance;
    }

    public void ReloadManager()
    {
        Awake();
    }

    private void Awake()
    {
        //check if only one instance at a time
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            _instance = this;
        }        

        _audioManagerAudioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayMusic();
    }  

    private void PlayMusic()
    {
        _audioManagerAudioSource.Play();
    }

    public void SetAudioSource(AudioSource source, string soundName)
    {
        SoundData s = Array.Find(_sounds, sound => sound._name == soundName);
        if(s == null)
        {
            Debug.Log(source.transform.parent.name + " tried to setup for " + soundName + " but failed.");
            return;
        }
        
        float volumeMultiplier = s._soundType == SoundData.SoundType.MUSIC ? _musicVolumeMultiplier.value : _sfxVolumeMultiplier.value;

        source.clip = s._clip;
        source.loop = s._isLoop;
        source.volume = s._volume * volumeMultiplier;
        source.pitch = s._pitch;
        source.playOnAwake = false;
        source.spatialBlend = _spatialBlendValue;
    }

    public void PlayPitchedSound(AudioSource source, float minPitch = .9f, float maxPitch = 1.1f)
    {
        float pitch = UnityEngine.Random.Range(minPitch, maxPitch);

        source.pitch = pitch;
        source.Play();
    }
}
