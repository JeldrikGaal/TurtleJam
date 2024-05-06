using System;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public class ScoreManager : MonoBehaviour
{

    private const string LeaderboardId = "Romanesco"; // Do not change! Sensitive variable.
    int Offset { get; set; }
    int Limit { get; set; }

    
    [SerializeField] private bool rankingScreen = false; // Indication if this is ranking screen.
    [SerializeField] private string playerSignedIn; // stores username of signed in player, and serves as indication if there's a player signed in.
    
    async void Awake()
    {
        // TODO: TEMPPPP
        //await UnityServices.InitializeAsync();
        //await SignInAnonymously();
    }
    
    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerName);
            playerSignedIn = AuthenticationService.Instance.PlayerName;
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
            // Refresh button shows up. Linked to this function.
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void UpdateScore(double newScore)
    {
        double currentScore = await GetPlayerScore();

        if (currentScore != null && newScore > currentScore)
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, newScore);
        }
    }

    private async Task<double> GetPlayerScore()
    {
        return (await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId)).Score; 
    }
    
    private async void UpdateUsername(string newUsername)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(newUsername);
    }
    
    public async Task<Dictionary<string,double>> GetAllScores()
    {
        if(!rankingScreen) return null;
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);

        return ConvertScoresToDictionary(scoresResponse.Results);
    }

    public async Task<Dictionary<string,double>> GetTopScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions{ Offset = 0, Limit = 10 });

        return ConvertScoresToDictionary(scoresResponse.Results);
    }
    
    private Dictionary<string, double> ConvertScoresToDictionary(List<LeaderboardEntry> results)
    {
        Dictionary<string, double> allScores = new Dictionary<string, double>();

        foreach (var result in results)
        {
            allScores.Add(result.PlayerName, result.Score);
        }

        return allScores;
    }
    
    

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Return)) GetScores();
    }

    /*private async Task<LeaderboardEntry> GetPlayerScoreAsync()
    {
        try
        {
            var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(_id);
 
            return entry;
        }
        catch (LeaderboardsException exception)
        {
            Debug.LogError($"[Unity Leaderboards] {exception.Reason}: {exception.Message}");
 
            return null;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
 
            return null;
        }
    }*/

    
    /*public void SaveNewScore(string name, int score) 
    {
        scores.Add(name, score);
        SaveScores();
    }*/

    /*private void SaveScores()
    {
        if (scores != null)
        {
            
            // Convert the scores dictionary to a JSON string
            string scoresJson = JsonUtility.ToJson(scores);

            // Save the scores to player prefs
            PlayerPrefs.SetString("Scores", scoresJson);
            Debug.Log(scoresJson);
        }
    }*/
    
}
