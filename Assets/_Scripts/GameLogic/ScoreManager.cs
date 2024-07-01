using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
   
    [SerializeField] private bool rankingScreen = false; // Indication if this is ranking screen.
    [SerializeField] public string playerSignedIn; // stores username of signed in player, and serves as indication if there's a player signed in.
    private static GameManager gameManager;

    private void Awake ()
    {
        SceneManager.sceneLoaded += UpdateUsernameDisplayBanner;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= UpdateUsernameDisplayBanner;
    }

    private void UpdateUsernameDisplayBanner(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("ttt");
        Debug.Log(playerSignedIn);
        //playerSignedIn = PlayFabClientAPI.GetUserData(new GetUserDataRequest())
        Debug.Log(gameObject.GetComponent<UsernameBanner>());
        this.gameObject.GetComponent<UsernameBanner>()?.DisplayUsernameInBanner(playerSignedIn);
    }

    public void SetPlayerName(string username)
    {
        playerSignedIn = username;
        GetComponent<PlayFabManager>().Login(username);
        UpdateUsernameDisplayBanner(default, LoadSceneMode.Single);
    }
    
    public string GetPlayerName()
    {
        return playerSignedIn;
    }
    
}
