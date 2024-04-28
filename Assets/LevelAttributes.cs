using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelAttributes : MonoBehaviour
{
    [SerializeField] private DirectionsHolder Entrances;
    [SerializeField] private DirectionsHolder Exits;
    [SerializeField] private float _minimumDifficulty;

    [SerializeField] private GameObject HorizontalWall;
    [SerializeField] private GameObject VerticalWall;
    [SerializeField] private GameObject HorizontalDoor;
    [SerializeField] private GameObject VerticalDoor;
    
    private LevelController.Direction _entranceDirection;
    private LevelController.Direction _exitDirection; 
    
    private List<SpawnerController> _spawners;

    private List<LevelController.Direction> _availableDirections = new List<LevelController.Direction>()
    {
        LevelController.Direction.Up, LevelController.Direction.Down, LevelController.Direction.Left,
        LevelController.Direction.Right
    };
    
    private Vector2 _roomSize = new Vector2(64,36);
    

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

    private void ApplyDoors() 
    {
        foreach (var direction in _availableDirections)
        {
            if(direction==_exitDirection){ }
        }
    }

    private Vector2 GetWallPosFromDirection(LevelController.Direction direction)
    {
        switch (direction)
        {
            case LevelController.Direction.Up:
                return new Vector2(0, _roomSize.y / 2);
            case LevelController.Direction.Down:
                return new Vector2(0, -_roomSize.y / 2);
            case LevelController.Direction.Left:
                return new Vector2(-_roomSize.x / 2, 0);
            case LevelController.Direction.Right:
                return new Vector2(_roomSize.x / 2, 0);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private List<GameObject> GetWallObjectFromDirection(LevelController.Direction direction)
    {
        // RETURNS A LIST OF [Wall, Door]
        switch (direction)
        {
            case LevelController.Direction.Up:
                return new List<GameObject> {HorizontalWall, HorizontalDoor};
            case LevelController.Direction.Down:
                return new List<GameObject> {HorizontalWall, HorizontalDoor};
            case LevelController.Direction.Left:
                return new List<GameObject> {VerticalWall, VerticalDoor};
            case LevelController.Direction.Right:
                return new List<GameObject> {VerticalWall, VerticalDoor};
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsRoomEligible(int currentDifficulty, LevelController.Direction entranceDirection)
    {
        return currentDifficulty >= _minimumDifficulty && Entrances.Directions.Contains(entranceDirection);
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
