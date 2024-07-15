using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoLogic : MonoBehaviour
{
   private VideoPlayer _videoPlayer;
   [SerializeField] private string _sceneToLoad;
   
   private void Awake()
   {
      _videoPlayer = GetComponent<VideoPlayer>();
      _videoPlayer.url = System.IO.Path.Combine (Application.streamingAssetsPath,"RPGLOGO.mp4");
      StartCoroutine(PlayIntroVideo());
   }

   private IEnumerator PlayIntroVideo()
   {
      _videoPlayer.Play();
      yield return new WaitForSeconds(7f);
      SceneManager.LoadScene(_sceneToLoad);

   }
}
