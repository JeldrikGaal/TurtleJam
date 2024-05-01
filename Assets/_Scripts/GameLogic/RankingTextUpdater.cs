using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingTextUpdater : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    private TextMeshProUGUI _leaderboardTMP;

    private void Start()
    {
        _leaderboardTMP = GetComponent<TextMeshProUGUI>();
    }

    private async void DisplayLeaderboard()
    {
        Dictionary<string, double> topScores = await scoreManager.GetTopScores();
        foreach (var score in topScores)
        {
            _leaderboardTMP.text += score.Key + " - " + score.Value + "\n";
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L)) DisplayLeaderboard();
    }
}
