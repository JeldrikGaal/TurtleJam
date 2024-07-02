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
    }

    private void OnDestroy()
    {
        PlayFabManager.OnLeaderBoardRetrieved -= SaveLeaderBoardEntries;
    }

    void Start()
    {
        PlayFabManager.Instance.GetLeaderboard();
    }

    private void SaveLeaderBoardEntries( List<PlayerLeaderboardEntry> entries)
    {
        _leaderboardEntries = entries;
        DisplayEntries();
    }

    private void GetPlayerStats()
    {
        string playerName = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>().GetPlayerName();
        List<PlayerLeaderboardEntry> entries = _leaderboardEntries.Where(x => x.DisplayName == playerName).ToList();
        if (entries.Count > 0)
        {
            PlayerLeaderboardEntry entry = entries[0];
            _ownName.text = entry.DisplayName;
            _ownScore.text = entry.StatValue.ToString();
            _ownRank.text = (entry.Position + 1).ToString();
        }
        else
        {
            _ownName.text = "";
            _ownScore.text = "";
            _ownRank.text = "";
            Debug.LogError("No entry found for player: " + playerName);
        }
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

        GetPlayerStats();
    }
}
