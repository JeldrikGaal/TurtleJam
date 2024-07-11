using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreManager : MonoBehaviour
{
    private int _currentTotalScore;
    private int _bounceKillScore;
    private int _enemyKillScore;
    private int _roomClearedScore;
    private int _streakScore;
    
    [SerializeField] private int _enemyBasePoints;
    [SerializeField] private int _enemyScoreMod;
    [SerializeField] private int _streakScoreMod;
    [SerializeField] private int _bounceKillMod;
    [SerializeField] private int _roomsClearedMod;

    [SerializeField] private GameObject _indicatorObject;
    
    public static ScoreManager Instance;
    public static event Action<int> ScoreAdded;
    
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
        
        StreakLogic.BounceKillDetected           += ReactToBounceKill;
        EnemyController.EnemyDeathWithLocation   += ReactToEnemyKill;
        StreakLogic.StreakReached                += ReactToStreakExtended;
        ExitTrigger.roomExited                   += ReactToRoomCleared;
        
    }
    
    private void OnDestroy()
    {
        StreakLogic.BounceKillDetected          -= ReactToBounceKill;
        EnemyController.EnemyDeathWithLocation  -= ReactToEnemyKill;
        StreakLogic.StreakReached               -= ReactToStreakExtended;
        ExitTrigger.roomExited                  -= ReactToRoomCleared;
        
    }

    private void ReactToBounceKill()
    {
        int scoreToAdd = _bounceKillMod;
        _bounceKillScore += scoreToAdd;
        AddScore(scoreToAdd);   
    }

    private void ReactToEnemyKill(Vector3 position)
    {
        int scoreToAdd = _enemyBasePoints * _enemyScoreMod;
        _enemyKillScore += scoreToAdd;
        AddScore(scoreToAdd);

        scoreToAdd += StreakLogic.Instance.CurrentStreak() * _streakScoreMod;
        SpawnScorePopup(position, scoreToAdd);
    }

    private void SpawnScorePopup(Vector3 position, int score)
    {
        GameObject indicator = Instantiate(_indicatorObject, position, Quaternion.identity);
        StreakIndicator indicatorScript = indicator.GetComponent<StreakIndicator>();
        indicatorScript.Activate("+" + score, StreakLogic.Instance.GetCurrentStreakColor());
    }
    

    private void ReactToRoomCleared(ExitTrigger exitTrigger)
    {
        int scoreToAdd = _roomsClearedMod;
        _roomClearedScore += scoreToAdd;
        AddScore(scoreToAdd);   
    }

    private void ReactToStreakExtended(int streak)
    {
        int scoreToAdd = streak * _streakScoreMod;
        _streakScore += scoreToAdd;
        AddScore(scoreToAdd);   
    }

    private int GetCurrentScore()
    {
        return _currentTotalScore;
    }
    
    public List<int> GetEndScoreBreakdownList()
    {
        return new List<int>
        {
            _enemyKillScore,
            _streakScore,
            _roomClearedScore,
            _bounceKillScore
        };
    }
    
    public void SaveScoreForPlayer() 
    {
        PlayFabManager.Instance.SendLeaderboard(GetCurrentScore());
    }
    
    private void AddScore(int scoreToAdd)
    {
        _currentTotalScore += scoreToAdd;
        ScoreAdded?.Invoke(scoreToAdd);
        
    }

    public int GetCurrentDisplayScore()
    {
        return GetCurrentScore();
    }
}
