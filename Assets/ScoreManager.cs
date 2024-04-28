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

    private string LeaderboardId = "Romanesco";
    int Offset { get; set; }
    int Limit { get; set; }
    
    public Dictionary<string, int> scores = new Dictionary<string, int>() { };
    public string scoresString;
    
    async void Awake()
    {
        await UnityServices.InitializeAsync();
        await SignInAnonymously();
    }
    
    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
            // Refresh button shows up. Linked to this function.
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    
    public async void AddScore(string playerName, double score)
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, 102);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }
    
    public async void GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
    
    public async void GetPaginatedScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions{Offset = Offset, Limit = Limit});
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPlayerScore()
    {
        var scoreResponse = 
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
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

    private void LoadScores()
    {
        // Retrieve the scores JSON string from player prefs
        
        
        
        /*string scoresJson = PlayerPrefs.GetString("Scores");

        if (!string.IsNullOrEmpty(scoresJson))
        {
            Debug.Log("YYY");
            // Deserialize the scores JSON string back into a dictionary
            scores = JsonUtility.FromJson<Dictionary<string, int>>(scoresJson);
        }*/
    }

    public List<KeyValuePair<string, int>> GetRankedScores()
    {
        // Load scores from player prefs
        LoadScores();

        // Convert the scores dictionary to a list of key-value pairs
        List<KeyValuePair<string, int>> rankedScores = new List<KeyValuePair<string, int>>();

        if (scores != null)
        {
            rankedScores.AddRange(scores);

            // Sort scores in descending order based on the values
            rankedScores.Sort((a, b) => b.Value.CompareTo(a.Value));
        }

        // Return the sorted scores list
        return rankedScores;
    }

    public KeyValuePair<string, int> GetHighestScore()
    {
        // Load scores from player prefs
        LoadScores();

        // Initialize variables for the highest score
        string highestName = string.Empty;
        int highestScore = 0;

        if (scores != null)
        {
            // Iterate through the scores dictionary to find the highest score
            foreach (KeyValuePair<string, int> score in scores)
            {
                if (score.Value > highestScore)
                {
                    highestName = score.Key;
                    highestScore = score.Value;
                }
            }
        }

        // Return the highest score as a key-value pair
        return new KeyValuePair<string, int>(highestName, highestScore);
    }
}
