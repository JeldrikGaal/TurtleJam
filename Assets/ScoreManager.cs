using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public Dictionary<string, int> scores = new Dictionary<string, int>() { };

    private void Start()
    {
        /*// Example scores for testing
        scores.Add("Mona", 50);
        scores.Add("Balduin", 75);
        scores.Add("Jeldrik with a CK", 0);
        scores.Add("Loai the king", 9899);*/

        SaveScores();
        LoadScores();
        //Debug.Log(scores);
    }

    public void SaveNewScore(string name, int score) 
    {
        scores.Add(name, score);
        SaveScores();
    }

    private void SaveScores()
    {
        if (scores != null)
        {
            
            // Convert the scores dictionary to a JSON string
            string scoresJson = JsonUtility.ToJson(scores);

            // Save the scores to player prefs
            PlayerPrefs.SetString("Scores", scoresJson);
            Debug.Log(scoresJson);
        }
    }

    private void LoadScores()
    {
        // Retrieve the scores JSON string from player prefs
        string scoresJson = PlayerPrefs.GetString("Scores");

        if (!string.IsNullOrEmpty(scoresJson))
        {
            Debug.Log("YYY");
            // Deserialize the scores JSON string back into a dictionary
            scores = JsonUtility.FromJson<Dictionary<string, int>>(scoresJson);
        }
    }

    public List<KeyValuePair<string, int>> GetRankedScores()
    {
        // Load scores from player prefs
        LoadScores();

        // Convert the scores dictionary to a list of key-value pairs
        List<KeyValuePair<string, int>> rankedScores = new List<KeyValuePair<string, int>>();

        if (scores != null)
        {
            rankedScores.AddRange(scores);

            // Sort scores in descending order based on the values
            rankedScores.Sort((a, b) => b.Value.CompareTo(a.Value));
        }

        // Return the sorted scores list
        return rankedScores;
    }

    public KeyValuePair<string, int> GetHighestScore()
    {
        // Load scores from player prefs
        LoadScores();

        // Initialize variables for the highest score
        string highestName = string.Empty;
        int highestScore = 0;

        if (scores != null)
        {
            // Iterate through the scores dictionary to find the highest score
            foreach (KeyValuePair<string, int> score in scores)
            {
                if (score.Value > highestScore)
                {
                    highestName = score.Key;
                    highestScore = score.Value;
                }
            }
        }

        // Return the highest score as a key-value pair
        return new KeyValuePair<string, int>(highestName, highestScore);
    }
}
