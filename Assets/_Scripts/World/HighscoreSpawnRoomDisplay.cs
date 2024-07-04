using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class HighscoreSpawnRoomDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _highscoreTextDisplay;

    [SerializeField] private string _defaultText;

    private void Awake()
    {
        PlayFabManager.OnHighestScoreRetrieved += SetHighScoreDisplay;
    }

    private void OnDestroy()
    {
        PlayFabManager.OnHighestScoreRetrieved -= SetHighScoreDisplay;
    }

    private void SetHighScoreDisplay(GetLeaderboardResult result)
    {
        _highscoreTextDisplay.text = _defaultText + result.Leaderboard[0].StatValue + "\n by: " + result.Leaderboard[0].DisplayName;
    }
    
    void Start()
    {
        PlayFabManager.Instance.GetHighestScore();
    }

  
}
