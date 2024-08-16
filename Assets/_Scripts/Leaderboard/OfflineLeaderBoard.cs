using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using UnityEngine;

public class OfflineLeaderBoard : MonoBehaviour
{
    public static OfflineLeaderBoard Instance;

    private List<PlayerLeaderboardEntry> _leaderBoardSave = new List<PlayerLeaderboardEntry>();
    
    private string _loggedInUsername;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        _leaderBoardSave = LoadLeaderboardFromFile();
    }
    
    public void Login(string username)
    {
        _loggedInUsername = username;
        LeaderBoardManager.Instance.InvokePlayerLoggedIn(username);
    }

    public string GetUserName()
    {
        return _loggedInUsername;
    }

    public void SendLeaderboard(int score)
    {
        AddPlayerToLeaderboard(_loggedInUsername, score);
    }

    public void GetPlayerInfo()
    {
        PlayerLeaderboardEntry constructedEntry = new PlayerLeaderboardEntry
        {
            DisplayName = GetUserName(),
            StatValue = GetHighScoreForPlayer(GetUserName()),
            Position = GetPositionForPlayer(GetUserName())
        };
        
        GetLeaderboardAroundPlayerResult constructedResult = new GetLeaderboardAroundPlayerResult();
        constructedResult.Leaderboard = new List<PlayerLeaderboardEntry> { constructedEntry };

        LeaderBoardManager.Instance.InvokeLeaderBoardAroundPlayGot(constructedResult);
    }

    private int GetPositionForPlayer(string getUserName)
    {
        return GetPlayer(getUserName).Position;
    }

    private int GetHighScoreForPlayer(string getUserName)
    {
        return GetPlayer(getUserName).StatValue;
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "leaderboard.json");
    }

    private void SaveLeaderboardToFile(List<PlayerLeaderboardEntry> leaderboard)
    {
        string json = JsonConvert.SerializeObject(leaderboard, Formatting.Indented);
        File.WriteAllText(GetFilePath(), json);
    }

    private List<PlayerLeaderboardEntry> LoadLeaderboardFromFile()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var deserializedList = JsonConvert.DeserializeObject<List<PlayerLeaderboardEntry>>(json);
            if (deserializedList == null)
            {
                return new List<PlayerLeaderboardEntry>();
            }
            else
            {
                return deserializedList;
            }
        }
        else
        {
            File.Create(filePath);
            return new List<PlayerLeaderboardEntry>();
        }
    }
    
    private void AddPlayerToLeaderboard(string playerName, int score)
    {
        PlayerLeaderboardEntry playerEntry = _leaderBoardSave.Find(entry => entry.DisplayName == playerName);
        if (playerEntry == null)
        {
            playerEntry = new PlayerLeaderboardEntry
            {
                DisplayName = playerName,
                StatValue = score
            };
            _leaderBoardSave.Add(playerEntry);
        }
        else
        {
            UpdatePlayerScore(playerName, score);
        }
        AssignPositions();
    }

    private PlayerLeaderboardEntry GetPlayer(string userName)
    {
        return _leaderBoardSave.Find(entry => entry.DisplayName == userName);
    }

    private void UpdatePlayerScore(string playerName, int newScore)
    {
        PlayerLeaderboardEntry playerEntry = _leaderBoardSave.Find(entry => entry.DisplayName == playerName);
        if (playerEntry != null)
        {
            playerEntry.StatValue = newScore;
        }
    }
    
    private void AssignPositions()
    {
        _leaderBoardSave.Sort((entry1, entry2) => entry2.StatValue.CompareTo(entry1.StatValue));
        for (int i = 0; i < _leaderBoardSave.Count; i++)
        {
            _leaderBoardSave[i].Position = i + 1;
        }
        SaveLeaderboardToFile(_leaderBoardSave);
    }
    
    public void GetLeaderboard()
    {
        _leaderBoardSave = LoadLeaderboardFromFile();
        _leaderBoardSave.Sort((entry1, entry2) => entry1.Position.CompareTo(entry2.Position));
        if (_leaderBoardSave.Count > 10)
        {
            _leaderBoardSave = _leaderBoardSave.GetRange(0, 10);
        }
        LeaderBoardManager.Instance.InvokeLeaderBoardGot(_leaderBoardSave);
    }
    
    public void GetHighestScore()
    {
        
        List<PlayerLeaderboardEntry> leaderboard = LoadLeaderboardFromFile();
        if (leaderboard.Count == 0)
        {
            LeaderBoardManager.Instance.InvokeHighestScoreGot(null);
            return;
        }
        leaderboard.Sort((entry1, entry2) => entry2.StatValue.CompareTo(entry1.StatValue));
        PlayerLeaderboardEntry highestScoreEntry = leaderboard[0];
        GetLeaderboardResult result = new GetLeaderboardResult
        {
            Leaderboard = new List<PlayerLeaderboardEntry> { highestScoreEntry }
        };
        LeaderBoardManager.Instance.InvokeHighestScoreGot(result);
    }
}
