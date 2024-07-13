using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingLogic : MonoBehaviour
{ 
    [SerializeField] private Sprite _musicOnSprite;
    [SerializeField] private Sprite _musicOffSprite;
    [SerializeField] private Sprite _vfxOnSprite;
    [SerializeField] private Sprite _vfxOffSprite;

    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _sfxButton;

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
        _sfxEnabled = SoundManager.GetSfxAllowed();
        _musicEnabled = SoundManager.IsMusicPlaying();
        
        _musicButtonRenderer = _musicButton.transform.GetComponent<Image>();
        _sfxButtonRenderer = _sfxButton.transform.GetComponent<Image>();

        SetSprites();
    }

    private void SetSprites()
    {
        _sfxButtonRenderer.sprite = _sfxEnabled ? _vfxOnSprite : _vfxOffSprite;
        _musicButtonRenderer.sprite = _musicEnabled ? _musicOnSprite : _musicOffSprite;
    }
    
    public void ToggleVfx()
    {
        _sfxEnabled = !_sfxEnabled;
        SoundManager.SetSfxAllowed(_sfxEnabled);
        SetSprites();
    }

    public void ToggleMusic()
    {
         _musicEnabled = SoundManager.ToggleMusic();
        SetSprites();
    }
    
    
}
