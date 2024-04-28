using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelAttributes : MonoBehaviour
{
    public List<LevelController.Direction> AvailableEntrances;
    public List<LevelController.Direction> AvailableExits;
    
    private LevelController.Direction _entranceDirection;
    private LevelController.Direction _exitDirection; 
    
    private List<SpawnerController> _spawners;
    
    [SerializeField] private Vector2 _roomSize = new Vector2(64,36);
    [SerializeField] private float _minimumDifficulty;

    private void Start()
    {
        _spawners = GetComponentsInChildren<SpawnerController>().ToList();
    }

    public void InitializeRoom(LevelController.Direction entranceDirection, LevelController.Direction exitDirection, int currentDifficulty)
    {
        _entranceDirection = entranceDirection;
        _exitDirection = exitDirection;

        ApplyDoors();
        InitializeSpawners(currentDifficulty);
    }

    private void InitializeSpawners(int currentDifficulty) // Check later
    {
        // TODO: temp fix remove ! 
        if (_spawners == null)
        {
            return;
        }
        foreach (var spawner in _spawners)
        {
            spawner.InitializeSpawner();
        }
    }

    private void ApplyDoors() // Check later
    {
        // fur Spater
    }

    public bool IsRoomEligible(int currentDifficulty, LevelController.Direction entranceDirection)
    {
        return currentDifficulty >= _minimumDifficulty && AvailableEntrances.Contains(entranceDirection);
    }

    public LevelController.Direction GetExitDirection()
    {
        return _exitDirection;
    }
    
    public Vector2 GetRoomSize()
    {
        return _roomSize;
    }
}
