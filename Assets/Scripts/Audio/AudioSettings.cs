using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] public Slider _musicVolumeSlider;
    [SerializeField] public Slider _sfxVolumeSlider;

    [SerializeField] public FloatVariable _musicVolumeMultiplier;
    [SerializeField] public FloatVariable _sfxVolumeMultiplier;    

    public void SetMusicVolume()
    {
        _musicVolumeMultiplier.value = _musicVolumeSlider.value;

        //for (int i = 0; i < AudioManager.GetInstance()._sounds.Length; i++)
        //{
        //    if (AudioManager.GetInstance()._sounds[i]._soundType == SoundData.SoundType.MUSIC)
        //    {
        //        AudioManager.GetInstance()._sounds[i]._source.volume = AudioManager.GetInstance()._sounds[i]._baseVolume * _musicVolumeMultiplier.value;
        //    }
        //}
    }

    public void SetSFXVolume()
    {
        _sfxVolumeMultiplier.value = _sfxVolumeSlider.value;
    }
}
