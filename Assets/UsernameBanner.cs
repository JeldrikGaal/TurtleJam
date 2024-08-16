using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UsernameBanner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _usernameText;

    private void Awake()
    {
        LeaderBoardManager.PlayerLoggedIn += UpdateUsernameBanner;
    }
    private void OnDestroy()
    {
        LeaderBoardManager.PlayerLoggedIn -= UpdateUsernameBanner;
    }

    private void Start()
    {
        Invoke(nameof(UpdateUsernameBanner), 0.1f);
    }

    private void UpdateUsernameBanner(string userName)
    {
        _usernameText.text = userName;
    }

    private void UpdateUsernameBanner(Scene arg0, LoadSceneMode arg1)
    {
        UpdateUsernameBanner();
    }
    
    private void UpdateUsernameBanner()
    {
        _usernameText.text = LeaderBoardManager.Instance.GetUserName();
    }

   

   

    
    
    
}
