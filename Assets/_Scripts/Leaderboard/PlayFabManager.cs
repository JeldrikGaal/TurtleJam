using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{

    private static PlayFabManager _instance;

    public static PlayFabManager Instance => _instance;

    private string _loggedInDisplayName = "";
    private string _loggedInUserID = "";

    private string _selectedLeaderBoard = "SteamBuild";

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
    
    private void OnEnable()
    {
        DEVHACKS.OnForceRestart += Reset;
    }

    private void OnDestroy()
    {
        DEVHACKS.OnForceRestart -= Reset;
    }

    private void Reset()
    {
        _loggedInDisplayName = "";
        _loggedInUserID = "";
        _selectedLeaderBoard = "SteamBuild";
    }

    public void Login(string username)
    {
        _loggedInUserID = username + SystemInfo.deviceUniqueIdentifier;
        _loggedInDisplayName = username;
        Debug.Log("Logging in with CustomID: " + _loggedInUserID);
        Debug.Log("Logging in with Username: " + _loggedInDisplayName);
        var request = new LoginWithCustomIDRequest
        {
            CustomId = username + SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

    }

    public string GetUserName()
    {
        return _loggedInDisplayName;
    }
    
    public string GetUserID()
    {
        return _loggedInUserID;
    }

    private void UpdatePlayFabUsername()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = GetUserName()
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateSuccess, OnUpdateFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you have logged in successfully!");
        Debug.Log("Your CustomID: " + SystemInfo.deviceUniqueIdentifier);
        UpdatePlayFabUsername();
        LeaderBoardManager.Instance.InvokePlayerLoggedIn(GetUserName());
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
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = _selectedLeaderBoard,
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

    public void GetPlayerInfo()
    {
        var request2 = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = _selectedLeaderBoard,
            MaxResultsCount = 1
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request2, OnLeaderboardAroundPlayerGet, OnLeaderboardError);
    }

    private void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult getLeaderboardAroundPlayerResult)
    {
        LeaderBoardManager.Instance.InvokeLeaderBoardAroundPlayGot(getLeaderboardAroundPlayerResult);
    }


    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = _selectedLeaderBoard,
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnLeaderboardError);
    }

    private void OnLeaderboardGet(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            //Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
        }

        LeaderBoardManager.Instance.InvokeLeaderBoardGot(result.Leaderboard);
    }

    public void GetHighestScore()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = _selectedLeaderBoard,
            StartPosition = 0,
            MaxResultsCount = 1
        };
        PlayFabClientAPI.GetLeaderboard(request, OnHighScoreGet, OnLeaderboardError);
    }

    private void OnHighScoreGet(GetLeaderboardResult result)
    {
        LeaderBoardManager.Instance.InvokeHighestScoreGot(result);
    }

    public void SwitchLeaderBoard(string newLeaderBoard)
    {
        _selectedLeaderBoard = newLeaderBoard;
    }
}
    