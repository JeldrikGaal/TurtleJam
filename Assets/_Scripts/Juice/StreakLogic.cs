using System;
using UnityEngine;

public class StreakLogic : MonoBehaviour
{
    [SerializeField] private GameObject _streakIndicator;
    
    private int _streakCount;
    private Action _receivedAction = Action.None;

    public static event Action<int> StreakReached;

    public static StreakLogic Instance;
    
    private enum Action
    {
        Shot,
        Kill,
        None
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        EnemyController.EnemyDeathWithLocation      += ReactToEnemyDeath;
        PlayerProjectile.ProjectileShot += ReactToShot;
        PlayerProjectile.ProjectileReturning += ReactToReturn;
    }

    private void OnDestroy()
    {
        EnemyController.EnemyDeathWithLocation      -= ReactToEnemyDeath;
        PlayerProjectile.ProjectileShot -= ReactToShot;
        PlayerProjectile.ProjectileReturning -= ReactToReturn;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetStreakCount(_streakCount + 10);
        }
    }

    private void ReactToShot()
    {
        _receivedAction = Action.Shot;
    }

    private void ReactToReturn()
    {
        if (_receivedAction == Action.Shot)
        {
            EndStreak();
        }
    }

    private void ReactToEnemyDeath(Vector3 pos)
    {
        if (_streakCount == 0)
        {
            StartStreak(pos);
        }
        else
        {
            ExtendStreak(pos);
        }
        
        _receivedAction = Action.Kill;
    }

    private void StartStreak(Vector3 pos)
    {
        SetStreakCount(1);
        SpawnIndicator(pos);
    }

    private void ExtendStreak(Vector3 pos)
    {
        SetStreakCount(_streakCount + 1);
        SpawnIndicator(pos);
    }

    private void EndStreak()
    {
        if (_streakCount > 3)
        {
            SoundManager.PlayOneShotSound(SoundManager.Sound.LoseStreak);
            SpawnIndicator(PlayerController.Instance.transform.position + new Vector3(0 , 0.75f, 0), true);
        }
        SetStreakCount(0);
        
    }

    private void SetStreakCount(int newStreakCount)
    {
        _streakCount = newStreakCount;
        StreakReached?.Invoke(_streakCount);
    }

    private void SpawnIndicator(Vector3 pos, bool streakEnded = false)
    {
        GameObject indicator = Instantiate(_streakIndicator, pos, Quaternion.identity);
        indicator.transform.position = pos;
        StreakIndicator indicatorScript = indicator.GetComponent<StreakIndicator>();
        indicatorScript.Activate(_streakCount, streakEnded);
    }

    public int CurrentStreak()
    {
        return _streakCount;
    }
}
