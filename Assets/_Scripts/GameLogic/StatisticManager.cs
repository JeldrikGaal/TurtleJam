using System.Collections.Generic;
using ByteBrewSDK;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    public class Statistics
    {
        public int EnemiesKilled = 0;
        public int RoomsCleared = 0;
        public int ShotsFired = 0;
        public int HighestStreak = 0;
    }

    private Statistics _statistics = new Statistics();
    public static StatisticManager Instance;
    private void Awake()
    {
        _statistics = new Statistics();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        EnemyController.EnemyDeath += AddKilledEnemy;
        ExitTrigger.roomExited += AddRoomCleared;
        PlayerProjectile.ProjectileShot += AddShotFired;
        StreakLogic.StreakReached += UpdateHighestStreak;
    }

    private void OnDestroy()
    {
        EnemyController.EnemyDeath -= AddKilledEnemy;
        ExitTrigger.roomExited -= AddRoomCleared;
        PlayerProjectile.ProjectileShot -= AddShotFired;
        StreakLogic.StreakReached -= UpdateHighestStreak;
    }

    private void AddKilledEnemy()
    {
        _statistics.EnemiesKilled += 1;
    }
    
    private void AddRoomCleared(ExitTrigger t)
    {
        _statistics.RoomsCleared += 1;
    }
    
    private void AddShotFired()
    {
        _statistics.ShotsFired += 1;
    }

    public Statistics GetStatistics()
    {
        return _statistics;
    }

    private void UpdateHighestStreak(int streak)
    {
        if (streak > _statistics.HighestStreak)
        {
            _statistics.HighestStreak = streak;
        }
    }
    
    public void RegisterAnalytics()
    {

        //string username = GameObject.FindWithTag("UnityPlugin").GetComponent<ScoreManager>().GetPlayerName();
        string round_time = (GetComponent<GameManager>()._timeSinceGameStarted).ToString();
        Statistics round_stats = GetStatistics();
        
        //Custom Event: ByteBrew
        Dictionary<string, string> roundCompleteParameters = new Dictionary<string, string>()
        {
            { "kills", round_stats.EnemiesKilled.ToString() },
            { "rooms_cleared", round_stats.RoomsCleared.ToString() },
            { "shots_fired", round_stats.ShotsFired.ToString() },
            { "time", round_time },
            { "username", "dev  " }
        };
        
        ByteBrew.NewCustomEvent("roundComplete", roundCompleteParameters);
        
        /*ByteBrew.NewCustomEvent("roundTime",GetComponent<GameManager>()._timeSinceGameStarted);
        ByteBrew.NewCustomEvent("enemies_killed",round_stats.EnemiesKilled);
        ByteBrew.NewCustomEvent("rooms_cleared",round_stats.RoomsCleared);
        ByteBrew.NewCustomEvent("shots_fired",round_stats.ShotsFired);
        ByteBrew.NewCustomEvent("username",username);*/
    }
    
}
