using System;
using System.Collections;
using System.Collections.Generic;
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

    private void SetHighScoreDisplay(string highestScore)
    {
        _highscoreTextDisplay.text = _defaultText + highestScore;
    }
    
    void Start()
    {
        PlayFabManager.Instance.GetHighestScore();
    }

  
}
