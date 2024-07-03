using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingTextUpdater : MonoBehaviour
{
    private TextMeshProUGUI _leaderboardTMP;

    [SerializeField] private Transform namesList;
    [SerializeField] private Transform scoresList;

    private void Start()
    {
        _leaderboardTMP = GetComponent<TextMeshProUGUI>();
    }

    private async void DisplayLeaderboard()
    {
        //Dictionary<string, double> topScores = await scoreManager.GetTopScores();
        //PopulateNames(topScores);
        //PopulateScores(topScores);
    }

    private void PopulateNames(Dictionary<string, double> topScores)
    {
        List<string> names = new List<string>();
        foreach (var score in topScores)
        {
            names.Add(score.Key);
        }

        foreach (TextMeshProUGUI child in namesList.GetComponentsInChildren<TextMeshProUGUI>())
        {
            child.text = names[child.GetComponentIndex()];
        }
    }
    
    private void PopulateScores(Dictionary<string, double> topScores)
    {
        List<double> scores = new List<double>();
        foreach (var score in topScores)
        {
            scores.Add(score.Value);
        }

        foreach (TextMeshProUGUI child in scoresList.GetComponentsInChildren<TextMeshProUGUI>())
        {
            child.text = scores[child.GetComponentIndex()].ToString();
        }
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.L)) DisplayLeaderboard();
    }
}
