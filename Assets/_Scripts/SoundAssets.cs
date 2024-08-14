using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundAssets : MonoBehaviour
{
    private static SoundAssets _i;

    public static SoundAssets i{
        get { 
            if( _i == null ) _i =Instantiate(Resources.Load<SoundAssets>("SoundAssets"));
            return _i; 
            }
    }
    public AudioMixer Mixer;
    public AudioMixerGroup Mixer_Master;
    public AudioMixerGroup Mixer_Music;
    public AudioMixerGroup Mixer_SFX;
    public SoundAudioClip[] soundAudioClips;
    public SoundAudioClips[] soundAudioClipsList;

    [System.Serializable]
    public class SoundAudioClip{
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
    [System.Serializable]
    public class SoundAudioClips {
        public SoundManager.Sound sound;
        public AudioClip[] audioClips;
    }

}
