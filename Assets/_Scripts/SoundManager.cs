using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class SoundManager 
{
    public enum Sound {
        PlayerProjectileFire,
        PlayerProjectileHit,
        PlayerShieldOpen,
        PlayerShieldHit,
        PlayerDeath,
        EnemyShoot,
        EnemyMove,
        EnemyDeath,
        Interactable,
        StartGame,
        Pause,
        GameOver,
        ButtonSelect,
        Music,
        LoseStreak,
        GainStreak,
        LoginPass,
        LoginFail
    }

    private static Dictionary<Sound, float> _soundTimer;
    private static GameObject _oneShotSoundGameObject;
    private static AudioSource _oneShotAudioSource;
    private static List<GameObject> _playingSounds;

    private static GameObject _musicGameObject;
    
    private static bool _sfxAllowed = true;

    public static void Initialize(){
        _soundTimer = new Dictionary<Sound, float>
        {
            [Sound.EnemyMove] = 0f
        };
    }

    public static void PlayOneShotSound(Sound sound, Vector3 position)
    {
        if (!_sfxAllowed)
        {
            return;   
        }
        
        if (!CanPlaySound(sound))
        {
            return;
        }
        
        GameObject soundGameObject = new GameObject("3DSound")
        {
            transform =
            {
                position = position
            }
        };
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0;
        audioSource.Play();
        Object.Destroy(soundGameObject, audioSource.clip.length);
    }

    public static void PlayOneShotSound(Sound sound){

        if (!_sfxAllowed)
        {
            return;   
        }
        
        if (_oneShotSoundGameObject == null)
        {
             _oneShotSoundGameObject = new GameObject("OneShotSound");
             _oneShotAudioSource = _oneShotSoundGameObject.AddComponent<AudioSource>();
        }
        if (CanPlaySound(sound))
        {
            _oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    private static GameObject CreateAndActivateSoundGameObject(Sound sound, Transform parent)
    {
        GameObject soundGameObject = new GameObject(sound.ToString())
        {
            transform = { parent = parent }
        };
        
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.loop = true;
        audioSource.Play();
        
        return soundGameObject;
    }
    
    public static void PlaySound(Sound sound, Transform parent)
    {
        if (_playingSounds == null)
        {
            _playingSounds = new List<GameObject>();
        }
        _playingSounds.Add(CreateAndActivateSoundGameObject(sound, parent));
    }
    
    public static void StopSound(Sound sound, Transform parent)
    {
        if (_playingSounds is not { Count: > 0 })
        {
            return;
        }
        foreach(GameObject soundGameObject in _playingSounds)
        {
            if(soundGameObject.name == sound.ToString() && soundGameObject.transform.parent == parent)
            {
                soundGameObject.GetComponent<AudioSource>().Stop(); 
                Object.Destroy(soundGameObject);
                _playingSounds.Remove(soundGameObject);
                return;
            }    
        }
    }

    public static void PlayMusic()
    {
        if (! _musicGameObject)
        {
            _musicGameObject = CreateAndActivateSoundGameObject(Sound.Music, GameManager.Instance.transform);
        }
        else
        {
            _musicGameObject.GetComponent<AudioSource>().Play();
        }
    }

    private static void PauseMusic()
    {
        _musicGameObject.GetComponent<AudioSource>().Pause();
    }
    
    private static void StopMusic()
    {
        _musicGameObject.GetComponent<AudioSource>().Stop();
    }
    
    private static bool CanPlaySound(Sound sound)
    {
        switch(sound){
            default:
                return true;
            case Sound.EnemyMove:
                if(_soundTimer.ContainsKey(sound))
                {
                    float lastTimePlayed = _soundTimer[sound];
                    float enemyMoveTimerMax = 0.5f;
                    if( lastTimePlayed + enemyMoveTimerMax < Time.time)
                    {
                        _soundTimer[sound] = Time.time;
                        return true;
                    }
                    return false;
                }
                return true;
        }
    }
    
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach(SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.soundAudioClips)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            } 
        }
        Debug.LogError("Sound" + sound + " not found");
        return null;

    }
    
    public static void StopAllPlayingSounds()
    {
        if (_playingSounds is not { Count: > 0 })
        {
            return;
        }
        foreach(GameObject soundGameObject in _playingSounds)
        {
            if(soundGameObject!=null) 
            {
                soundGameObject.GetComponent<AudioSource>().Stop(); 
                Object.Destroy(soundGameObject);
            }
        }
        
        StopMusic();
        Reset();
    }
    
    public static void PauseAllPlayingSounds()
    {
        if(_playingSounds is { Count: > 0 })
        {
            foreach(GameObject soundGameObject in _playingSounds)
            {
                if (soundGameObject != null)
                {
                    soundGameObject.GetComponent<AudioSource>().Pause(); 
                }
            }
        }
    }
    
    public static void ResumeAllPlayingSounds()
    {
        if(_playingSounds is { Count: > 0 })
        {
            foreach(GameObject soundGameObject in _playingSounds)
            {
                if (soundGameObject != null)
                {
                    soundGameObject.GetComponent<AudioSource>().Play(); 
                }
            }
        }
    }
    
    public static void Reset()
    {
        _playingSounds?.Clear();
        _musicGameObject = null;
    }

    public static bool IsMusicPlaying()
    {
        if (!_musicGameObject)
        {
            return false;
        }
        else
        {
            return _musicGameObject.GetComponent<AudioSource>().isPlaying;
        }
    }
    
    public static bool GetSfxAllowed()
    {
        return _sfxAllowed;
    }

    public static void SetSfxAllowed(bool allowed)
    {
        _sfxAllowed = allowed;
        if (allowed)
        {
            ResumeAllPlayingSounds();
        }
    }

    public static bool ToggleSfx()
    {
        SetSfxAllowed(!_sfxAllowed);
        return _sfxAllowed;
    }

    public static bool ToggleMusic()
    {
        if (GameManager.Instance == null)
        {
            return false;
        }
        
        if (_musicGameObject == null)
        {
            PlayMusic();
            return true;
        }

        if (_musicGameObject.GetComponent<AudioSource>().isPlaying)
        {
            PauseMusic();
            return false;
        }

        PlayMusic();
        return true;
    }

    

}
