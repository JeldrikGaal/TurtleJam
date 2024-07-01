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
        GainStreak


    }

    private static Dictionary<Sound, float> soundTimer;
    private static GameObject oneShotSoundGameObject;
    private static AudioSource oneShotAudioSource;
    private static List<GameObject> playingSounds;

    public static void Initialize(){
        soundTimer = new Dictionary<Sound, float>();
        soundTimer[Sound.EnemyMove] = 0f;
    }

    public static void PlayOneShotSound(Sound sound, Vector3 position){
        if(CanPlaySound(sound)){
            GameObject soundGameObject = new GameObject("3DSound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0;
            audioSource.Play();
            Object.Destroy(soundGameObject, audioSource.clip.length);

        }
    }

    public static void PlayOneShotSound(Sound sound){
        if(oneShotSoundGameObject == null){
         oneShotSoundGameObject = new GameObject("OneShotSound");
         oneShotAudioSource = oneShotSoundGameObject.AddComponent<AudioSource>();
        }
        if(CanPlaySound(sound)){
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
        
    }

    public static void PlaySound(Sound sound, Transform parent)
    {

            GameObject soundGameObject = new GameObject(sound.ToString());
            soundGameObject.transform.parent = parent;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.loop = true;
            audioSource.Play();
            if(playingSounds==null)
            playingSounds = new List<GameObject>();
            playingSounds.Add(soundGameObject);
            //Object.Destroy(soundGameObject, audioSource.clip.length);

    }
    public static void StopSound(Sound sound, Transform parent)
    {
        if(playingSounds!=null && playingSounds.Count>0){
            foreach(GameObject soundGameObject in playingSounds){
            if(soundGameObject.name == sound.ToString() && soundGameObject.transform.parent == parent)
            {
                soundGameObject.GetComponent<AudioSource>().Stop(); 
                Object.Destroy(soundGameObject);
                playingSounds.Remove(soundGameObject);
                return;
            }    
        }

        }
    }
    private static bool CanPlaySound(Sound sound){
        switch(sound){
            default:
            return true;
            case Sound.EnemyMove:
            if(soundTimer.ContainsKey(sound)){
                float lastTimePlayed = soundTimer[sound];
                float enemyMoveTimerMax = 0.5f;
                if(lastTimePlayed + enemyMoveTimerMax < Time.time){
                    soundTimer[sound] = Time.time;
                    return true;
                }
                else return false;
            }
            else return true;
        }
    }
    private static AudioClip GetAudioClip(Sound sound){
        foreach(SoundAssets.SoundAudioClip soundAudioClip in SoundAssets.i.soundAudioClips){
            if(soundAudioClip.sound == sound){
                return soundAudioClip.audioClip;
            } 
        }
        Debug.LogError("Sound" + sound + " not found");
        return null;

    }
    public static void StopAllPlayingSounds(){
        if(playingSounds!=null && playingSounds.Count > 0){

        foreach(GameObject soundGameObject in playingSounds)
        {
            if(soundGameObject!=null){
            
            soundGameObject.GetComponent<AudioSource>().Stop(); 
            Object.Destroy(soundGameObject);

            }
            
        }
        playingSounds.Clear();

        }

    }
    public static void PauseAllPlayingSounds(){
        if(playingSounds!=null && playingSounds.Count > 0){

        foreach(GameObject soundGameObject in playingSounds)
        {
             if(soundGameObject!=null)
            soundGameObject.GetComponent<AudioSource>().Pause(); 
        }

        }

    }
    public static void ResumeAllPlayingSounds(){
        if(playingSounds!=null && playingSounds.Count > 0){

        foreach(GameObject soundGameObject in playingSounds)
        {
             if(soundGameObject!=null)
            soundGameObject.GetComponent<AudioSource>().Play(); 
        }

        }

    }
    public static void Reset()
    {
        if(playingSounds!=null){
            playingSounds.Clear();
        }
    }
    

}
