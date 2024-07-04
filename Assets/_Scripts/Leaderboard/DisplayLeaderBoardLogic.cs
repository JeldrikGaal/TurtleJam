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
    [SerializeField] private List<TMP_Text> _positions;

    [SerializeField] private TMP_Text _ownName;
    [SerializeField] private TMP_Text _ownScore;
    [SerializeField] private TMP_Text _ownRank;

    [SerializeField] private Color _ownColor;

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
        int pos = results.Leaderboard[0].Position;
        var entry = results.Leaderboard[0];
        if ( pos < 10)
        {
            _names[pos].text = entry.DisplayName;
            _ranking[pos].text = entry.StatValue.ToString();
            
            _names[pos].color = _ownColor;
            _ranking[pos].color = _ownColor;
            _positions[pos].color = _ownColor;
            
        }
        else
        {
            _ownName.text = entry.DisplayName;
            _ownScore.text = entry.StatValue.ToString();
            _ownRank.text = (entry.Position + 1).ToString();
        }
        
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
