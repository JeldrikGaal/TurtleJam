using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
   
    [SerializeField] private bool rankingScreen = false; // Indication if this is ranking screen.
    [SerializeField] private string playerSignedIn; // stores username of signed in player, and serves as indication if there's a player signed in.
    private static GameManager gameManager; 
    
    private void UpdateUsernameDisplayBanner()
    {
        this.gameObject.GetComponent<UsernameBanner>().DisplayUsernameInBanner(playerSignedIn);
    }

    public async Task UpdateScoresOnLeaderboard()
    {
        /*if (gameManager == null)
        {
            gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        }

        List<TextMeshProUGUI> namesPlaceholders = gameManager.GetComponent<LeaderboardDisplayConnector>().names;
        List<TextMeshProUGUI> scoresPlaceholders = gameManager.GetComponent<LeaderboardDisplayConnector>().scores;
        TextMeshProUGUI playerNamePlaceholder = gameManager.GetComponent<LeaderboardDisplayConnector>().playerName;
        TextMeshProUGUI playerScorePlaceholder = gameManager.GetComponent<LeaderboardDisplayConnector>().playerScore;
        
        Dictionary<string, double> allScores = await GetTopScores();
        int userCounter = 0;

        // Update Names and Scores
        foreach (var result in allScores)
        {
            namesPlaceholders[userCounter].text = result.Key; // Player Username
            scoresPlaceholders[userCounter].text = result.Value.ToString(); // Player Score
            userCounter++;
        }

        // Update Current Player Name and Score
        playerNamePlaceholder.text = playerSignedIn;
        playerScorePlaceholder.text = gameManager._score.ToString();*/
    }

    public void SetPlayerName(string username)
    {
        playerSignedIn = username;
        GetComponent<PlayFabManager>().Login(username);
        UpdateUsernameDisplayBanner();
    }
    
    public string GetPlayerName()
    {
        return playerSignedIn;
    }
    
}
