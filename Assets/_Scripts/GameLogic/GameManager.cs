using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int _score;
    private int _streakBonusScoreCount;
    private int _enemyScore;
    public float _timeSinceGameStarted;
    
    // Cursor
    [SerializeField] private Texture2D _cursorTexture;
    private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;
    
    [SerializeField] private GameObject _gameOverVisualEffectPrefab;
    
    [SerializeField] private float _timePlayedScoreMod;
    [SerializeField] private int _roomsClearedMod;
    [SerializeField] private int _scoreMod;
    [SerializeField] private Vector2 _roomBounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        InitializeValuesAndReferences();
        SetupCursor();
    }

    private void InitializeValuesAndReferences()
    {
        SoundManager.Reset();
        SoundManager.PlayOneShotSound(SoundManager.Sound.StartGame);
        SoundManager.PlaySound(SoundManager.Sound.Music, this.transform);
        Time.timeScale = 1;
        _score = 0;
        _timeSinceGameStarted = 0;
        
        //_scoreManager = GameObject.FindWithTag("UnityPlugin").GetComponent<ScoreManager>();
        
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.Running);
    }
    
    private void SetupCursor()
    {
        _hotSpot = new Vector2(_cursorTexture.width / 2f, _cursorTexture.height / 2f);
        Cursor.SetCursor(_cursorTexture, _hotSpot, CursorMode);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GameStateManager.Instance.IsRunning())
            {
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Paused);
            }
            else
            {
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Running);
            }
        }

        if (GameStateManager.Instance.IsPaused())
        {
            return;
        }
        
        _timeSinceGameStarted += Time.deltaTime;
    }

    public void ResumeGame()
    {
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.Running);
    }

    public float CalculateScore()
    {
        StatisticManager.Statistics stats = StatisticManager.Instance.GetStatistics();
        return (int)(_timeSinceGameStarted * _timePlayedScoreMod * 0f) + (_score * _scoreMod ) + (stats.RoomsCleared * _roomsClearedMod) ;
    }

    public List<int> GetEndScoreBreakdownList()
    {
        StatisticManager.Statistics stats = StatisticManager.Instance.GetStatistics();
        List<int> scoreBreakdown = new List<int>();
        scoreBreakdown.Add(_enemyScore * _scoreMod);
        scoreBreakdown.Add(_streakBonusScoreCount * _scoreMod);
        scoreBreakdown.Add(stats.RoomsCleared * _roomsClearedMod);
        scoreBreakdown.Add((int)(_timeSinceGameStarted * _timePlayedScoreMod));
        
       
        return scoreBreakdown;
    }
    
    public void SaveScoreForPlayer(float score) 
    {
        // TODO: Refactor ALL ScoreManager functions that are tied to UnityPLugin.
        //_scoreManager.UpdateScore(score);
        PlayFabManager.Instance.SendLeaderboard(Convert.ToInt32(score));
    }

    public void GoToLevel(string levelName) // For Resume, Next Level and Back to Main Menu
    {
        if (levelName == "this")
        {
            SoundManager.PlayOneShotSound(SoundManager.Sound.ButtonSelect);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        SceneManager.LoadScene(levelName);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }


    public void AddScore(int scoreToAdd)
    {
        int modification = (StreakLogic.Instance.CurrentStreak());
        _streakBonusScoreCount += modification;
        _enemyScore += scoreToAdd;
        _score += scoreToAdd + modification;
    }

    public Vector2 GetRoomBounds()
    {
        return _roomBounds;
    }
}
