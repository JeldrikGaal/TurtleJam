using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
   
   AudioClip RandomClip()
    {
        return audioClipArray[Random.Range(0, audioClipArray.Length)];
    }
   public void PlayOneShotSound()
    {
         audioSource.PlayOneShot(RandomClip());
    }


}
