using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    
    private const string MusicVolumeID = "MusicVolume";
    private const string SfxVolumeID = "SFXVolume";

    [SerializeField] private List<MixerVolumeDict> _musicDict;
    [SerializeField] private List<MixerVolumeDict> _vfXDict;

    [SerializeField] private BarDisplay _vfxDisplay;
    [SerializeField] private BarDisplay _musicDisplay;
    
    [Serializable]
    private struct MixerVolumeDict
    {
        public int VolumeAmount;
        public float MixerAmount;
    }
    
    private int _currentMusicAmount;
    private int _currentVfxAmount;

    [SerializeField] private int _maxAmount;
    
    private void Start()
    {
        _audioMixer = SoundAssets.i.Mixer;

        _currentMusicAmount = GetAmountFromVolume(_musicDict, GetMusicVolume());
        _currentVfxAmount = GetAmountFromVolume(_vfXDict, GetVfxVolume());
        
        _musicDisplay.Initialize(_currentMusicAmount);
        _vfxDisplay.Initialize(_currentVfxAmount);
    }

    public void IncreaseMusicVolume()
    {
        if (_currentMusicAmount >= _maxAmount)
        {
            return;
        }
        
        _currentMusicAmount++;
        SetMusicVolume(GetVolumeFromAmount(_musicDict, _currentMusicAmount));
        
        _musicDisplay.SetListFromAmountWithAnim(_currentMusicAmount);
    }

    public void DecreaseMusicVolume()
    {
        if (_currentMusicAmount < 1)
        {
            return;
        }
        
        _currentMusicAmount--;
        SetMusicVolume(GetVolumeFromAmount(_musicDict, _currentMusicAmount));
        
        _musicDisplay.SetListFromAmountWithAnim(_currentMusicAmount);
    }
    
    public void IncreaseVfxVolume()
    {
        if (_currentVfxAmount >= _maxAmount)
        {
            return;
        }
        
        _currentVfxAmount++;
        SetSfxVolume(GetVolumeFromAmount(_vfXDict, _currentVfxAmount));
        
        _vfxDisplay.SetListFromAmountWithAnim(_currentVfxAmount);
    }

    public void DecreaseVfxVolume()
    {
        if (_currentVfxAmount < 1)
        {
            return;
        }
        
        _currentVfxAmount--;
        SetSfxVolume(GetVolumeFromAmount(_vfXDict, _currentVfxAmount));
        
        _vfxDisplay.SetListFromAmountWithAnim(_currentVfxAmount);
    }

    private float GetVolumeFromAmount(List<MixerVolumeDict> map, int amount)
    {
        foreach (var data in map)
        {
            if (data.VolumeAmount == amount)
            {
                return data.MixerAmount;
            }
        }

        return -1;
    }

    private int GetAmountFromVolume(List<MixerVolumeDict> map, float volume)
    {
        foreach (var data in map)
        {
            if (Math.Abs(data.MixerAmount - volume) < 0.01f)
            {
                return data.VolumeAmount;
            }
        }

        return -1;
    }

    private void SetMusicVolume(float newValue)
    {
        _audioMixer.SetFloat(MusicVolumeID, newValue);
    }
    
    private void SetSfxVolume(float newValue)
    {
        _audioMixer.SetFloat(SfxVolumeID, newValue);
    }

    private float GetMusicVolume()
    {
        _audioMixer.GetFloat(MusicVolumeID, out var returnVal );
        return returnVal;
    }
    
    private float GetVfxVolume()
    {
        _audioMixer.GetFloat(SfxVolumeID, out var returnVal );
        return returnVal;
    }
}
