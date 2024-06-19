using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    public class Statistics
    {
        public int EnemiesKilled = 0;
        public int RoomsCleared = 0;
        public int ShotsFired = 0;
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
        CamUpTrigger.roomExited += AddRoomCleared;
        PlayerProjectile.ProjectileShot += AddShotFired;
    }

    private void OnDestroy()
    {
        EnemyController.EnemyDeath -= AddKilledEnemy;
        CamUpTrigger.roomExited -= AddRoomCleared;
        PlayerProjectile.ProjectileShot -= AddShotFired;
    }

    private void AddKilledEnemy()
    {
        _statistics.EnemiesKilled += 1;
    }
    
    private void AddRoomCleared(CamUpTrigger t)
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
    
    
}
