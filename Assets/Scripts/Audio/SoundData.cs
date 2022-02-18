using UnityEngine;

[System.Serializable]
public class SoundData
{
    public enum SoundType
    {
        MUSIC,
        SFX,
    }

    [SerializeField] public string _name;
    [SerializeField] public AudioClip _clip;
    [SerializeField] public SoundType _soundType;
    [SerializeField] public bool _isLoop;
    [SerializeField] public float _baseVolume = 1;
    [SerializeField] [Range(0, 1f)] public float _volume = 1;
    [SerializeField] [Range(.1f, 3f)] public float _pitch = 1;

    [HideInInspector] public AudioSource _source;
}
