using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayFabManager : MonoBehaviour
{
    
    private static PlayFabManager _instance;
    public static PlayFabManager Instance { get { return _instance; } }

    public static event Action<string> OnHighestScoreRetrieved;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("PlayFabManager can only have one running instance");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier, 
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        
    }

    public void UpdatePlayfabUsername()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = GetComponent<ScoreManager>().playerSignedIn
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateSuccess, OnUpdateFailure);
    }
    
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you have logged in successfully!");
        Debug.Log("Your CustomID: " + SystemInfo.deviceUniqueIdentifier);
        UpdatePlayfabUsername();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("LOGIN FAILURE:");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    
    private void OnUpdateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Congratulations, you have updated your username!");
        Debug.Log("Username: " + result.DisplayName);
    }
    
    private void OnUpdateFailure(PlayFabError error)
    {
        Debug.LogWarning("UPDATE DISPLAY NAME FAILURE:");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    
    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnLeaderboardError);
    }

    private void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful Leaderboard Entry!");
    }

    private void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogWarning("LEADERBOARD ENTRY FAILURE!");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnLeaderboardError);
    }
    
    private void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }
    
    public void GetHighestScore()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 1
        };
        PlayFabClientAPI.GetLeaderboard(request, OnHighScoreGet, OnLeaderboardError);
    }

    private void OnHighScoreGet(GetLeaderboardResult result)
    {
        OnHighestScoreRetrieved?.Invoke(result.Leaderboard[0].StatValue.ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetLeaderboard();
        }
    }

    /*public void UpdateUsername(string name)
    {

    }*/
}