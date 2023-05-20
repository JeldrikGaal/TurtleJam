using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> audioClip1 = new List<AudioClip>();
    public List<AudioClip> audioClip2 = new List<AudioClip>();
    public List<AudioClip> audioClip3 = new List<AudioClip>();
    public List<AudioClip> audioClip4 = new List<AudioClip>();
    public List<AudioClip> audioClip5 = new List<AudioClip>();


    public List<List<AudioClip>> audioClipList = new List<List<AudioClip>>();
    GameManager gM;
   AudioClip RandomClip(int id)
    {
        Debug.Log((id, audioClipList[id].Count));
        return audioClipList[id][Random.Range(0, audioClipList[id].Count)];
    }
   
    public void PlayOneShotSound()
    {
        audioSource.PlayOneShot(RandomClip(0));
    }
   public void PlayOneShotSound(int id)
    {
         audioSource.PlayOneShot(RandomClip(id));
    }

    public void Start()
    {

        gM = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioSource = gM.GetComponent<AudioSource>();
        audioClipList.Add(audioClip1);
        audioClipList.Add(audioClip2);
        audioClipList.Add(audioClip3);
        audioClipList.Add(audioClip4);
        audioClipList.Add(audioClip5);
    }

}
