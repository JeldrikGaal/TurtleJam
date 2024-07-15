using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettingLogic : MonoBehaviour
{ 
    [SerializeField] private Sprite _musicOnSprite;
    [SerializeField] private Sprite _musicOffSprite;
    [SerializeField] private Sprite _vfxOnSprite;
    [SerializeField] private Sprite _vfxOffSprite;

    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _sfxButton;
    private AudioMixer mixer;
    const string Mixer_Music = "MusicVolume";
    const string Mixer_SFX = "SFXVolume";

    private Image _musicButtonRenderer;
    private Image _sfxButtonRenderer;

    private bool _sfxEnabled;
    private bool _musicEnabled;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        mixer = SoundAssets.i.Mixer;
      
        _musicButtonRenderer = _musicButton.transform.GetComponent<Image>();
        _sfxButtonRenderer = _sfxButton.transform.GetComponent<Image>();
        ToggleVfx();
        ToggleMusic();

        //SetSprites();
    }

    private void SetSprites()
    {
        _sfxButtonRenderer.sprite = _sfxEnabled ? _vfxOnSprite : _vfxOffSprite;
        _musicButtonRenderer.sprite = _musicEnabled ? _musicOnSprite : _musicOffSprite;
    }
    
    public void ToggleVfx()
    {
        _sfxEnabled = !_sfxEnabled;
        if(_sfxEnabled)
        SetSFXVolume(0f);
        else
        SetSFXVolume(-80f);
        SetSprites();
    }

    public void ToggleMusic()
    {
         _musicEnabled = !_musicEnabled;
        if(_musicEnabled)
        SetMusicVolume(0f);
        else
        SetMusicVolume(-80f);
        SetSprites();
    }
    private void SetMusicVolume(float volume){
        mixer.SetFloat(Mixer_Music, Mathf.Log10(volume)*20f);

    }
    private void SetSFXVolume(float volume)
    {
        mixer.SetFloat(Mixer_SFX, Mathf.Log10(volume)*20f);

    }
    
    
}
