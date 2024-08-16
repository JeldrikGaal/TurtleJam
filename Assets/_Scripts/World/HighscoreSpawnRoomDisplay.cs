using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class HighscoreSpawnRoomDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _highscoreTextDisplay;

    [SerializeField] private string _defaultText;

    private void Awake()
    {
        LeaderBoardManager.OnHighestScoreRetrieved += SetHighScoreDisplay;
    }

    private void OnDestroy()
    {
        LeaderBoardManager.OnHighestScoreRetrieved -= SetHighScoreDisplay;
    }

    private void SetHighScoreDisplay(GetLeaderboardResult result)
    {
        _highscoreTextDisplay.text = _defaultText + result.Leaderboard[0].StatValue + "\n by: " + result.Leaderboard[0].DisplayName;
    }
    
    void Start()
    {
        LeaderBoardManager.Instance.GetHighestScore();
    }

  
}
