using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DisplayLeaderBoardLogic : MonoBehaviour
{
    private List<PlayerLeaderboardEntry> _leaderboardEntries;
    
    [SerializeField] private List<TMP_Text> _names;
    [SerializeField] private List<TMP_Text> _ranking;

    [SerializeField] private TMP_Text _ownName;
    [SerializeField] private TMP_Text _ownScore;
    [SerializeField] private TMP_Text _ownRank;

    private void Awake()
    {
        PlayFabManager.OnLeaderBoardRetrieved += SaveLeaderBoardEntries;
        PlayFabManager.OnLeaderBoardAroundPlayerRetrieved += ReactToPlayerDataRetrieved;
    }

    private void OnDestroy()
    {
        PlayFabManager.OnLeaderBoardRetrieved -= SaveLeaderBoardEntries;
        PlayFabManager.OnLeaderBoardAroundPlayerRetrieved -= ReactToPlayerDataRetrieved;
    }

    void Start()
    {
        PlayFabManager.Instance.GetLeaderboard();
    }

    private void SaveLeaderBoardEntries( List<PlayerLeaderboardEntry> entries)
    {
        _leaderboardEntries = entries;
        DisplayEntries();
        PlayFabManager.Instance.GetPlayerInfo();
    }

    private void ReactToPlayerDataRetrieved(GetLeaderboardAroundPlayerResult results)
    {
        _ownName.text = results.Leaderboard[0].DisplayName;
        _ownScore.text = results.Leaderboard[0].StatValue.ToString();
        _ownRank.text = (results.Leaderboard[0].Position + 1).ToString();
    }
    
    private void GetPlayerStats(GetPlayerStatisticsResult result)
    {
        /*result.Statistics[0].
        */

    }

    private void DisplayEntries()
    {
        
        for (int i = 0; i < _names.Count; i++)
        {
            _names[i].text = "";
            _ranking[i].text = "";
        }

        foreach (var entry in _leaderboardEntries)
        {
            if (entry.Position < 10)
            {
                _names[entry.Position].text = entry.DisplayName;
                _ranking[entry.Position].text = entry.StatValue.ToString();
            }
        }
    }
}
