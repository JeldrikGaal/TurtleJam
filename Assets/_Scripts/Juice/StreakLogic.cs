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
    }

    private void OnDestroy()
    {
        EnemyController.EnemyDeathWithLocation      -= ReactToEnemyDeath;
        PlayerProjectile.ProjectileShot -= ReactToShot;
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
        if (_receivedAction == Action.Shot)
        {
            EndStreak();
        }
        _receivedAction = Action.Shot;
        
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
        SetStreakCount(0);
    }

    private void SetStreakCount(int newStreakCount)
    {
        _streakCount = newStreakCount;
        StreakReached?.Invoke(_streakCount);
    }

    private void SpawnIndicator(Vector3 pos)
    {
        GameObject indicator = Instantiate(_streakIndicator, pos, Quaternion.identity);
        indicator.transform.position = pos;
        StreakIndicator indicatorScript = indicator.GetComponent<StreakIndicator>();
        indicatorScript.Activate(_streakCount);

    }

    public int CurrentStreak()
    {
        return _streakCount;
    }
}
