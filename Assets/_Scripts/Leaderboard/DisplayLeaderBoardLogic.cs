using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class DisplayLeaderBoardLogic : MonoBehaviour
{
    private List<PlayerLeaderboardEntry> _leaderboardEntries;
    
    [SerializeField] private List<TMP_Text> _names;
    [SerializeField] private List<TMP_Text> _ranking;

    [SerializeField] private TMP_Text _ownName;
    [SerializeField] private TMP_Text _ownScore;

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

    private void DisplayEntries()
    {
        
        for (int i = 0; i < _names.Count; i++)
        {
            _names[i].text = "";
            _ranking[i].text = "";
        }
        int count = 0;
        foreach (var entry in _leaderboardEntries)
        {
            //Debug.Log(entry.DisplayName + " : " + entry.Position + " : " + entry.StatValue);
            _names[count].text = entry.DisplayName;
            _ranking[count].text = entry.StatValue.ToString();
            count++;
        }
       
    }
}
