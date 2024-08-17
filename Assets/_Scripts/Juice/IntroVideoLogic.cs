using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoLogic : MonoBehaviour
{
   private VideoPlayer _videoPlayer;
   [SerializeField] private string _sceneToLoad;
   [SerializeField] private RawImage _warningImage;
   [SerializeField] private RawImage _videoImage;
   [SerializeField] private TMP_Text _pressAnyKeyText;

   private bool _started;
   private void Awake()
   {
      _videoPlayer = GetComponent<VideoPlayer>();
      _videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath,"RPGLOGO.mp4");
      
   }

   private void Update()
   {
      if (Input.anyKeyDown)
      {
         _pressAnyKeyText.gameObject.SetActive(false);
         StartCoroutine(PlayIntroVideo());
      }
      
      
   }

   private IEnumerator PlayIntroVideo()
   {
      if (_started)
      {
         yield break;
      }

      _started = true;
      
      _warningImage.DOFade(0, 0);
      _warningImage.DOFade(1, 1f);
      yield return new WaitForSeconds(4f);
      _warningImage.DOFade(0, 0.5f).OnComplete(() =>
      {
         _warningImage.gameObject.SetActive(false);   
      });
      yield return new WaitForSeconds(0.5f);
      
      _videoImage.gameObject.SetActive(true);
      _videoPlayer.Play();
      yield return new WaitForSeconds(7f);
      SceneManager.LoadScene(_sceneToLoad);

   }
}
