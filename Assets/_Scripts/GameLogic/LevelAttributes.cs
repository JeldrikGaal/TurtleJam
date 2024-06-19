using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private float _wallWidth = 1f;

    private List<LevelController.Direction> _availableDirections = new List<LevelController.Direction>()
    {
        LevelController.Direction.Up, LevelController.Direction.Down, LevelController.Direction.Left,
        LevelController.Direction.Right
    };
    
    private Vector2 _roomSize = new (32,18);
    private TransitionRoomDisplay _transitionRoomDisplay;

    private void Start()
    {
        _spawners = GetComponentsInChildren<SpawnerController>().ToList();
        
        if (TryGetComponent<TransitionRoomDisplay>(out var component))
        {
            _transitionRoomDisplay = component;
        }
        else
        {
            _transitionRoomDisplay = null;
        }
    }

    public void InitializeRoom(LevelController.Direction entranceDirection, LevelController.Direction exitDirection, int currentDifficulty)
    {
        _entranceDirection = entranceDirection;
        _exitDirection = exitDirection;

        ApplyDoors();
        InitializeSpawners();
    }

    private void InitializeSpawners() // Check later
    {
        _spawners = GetComponentsInChildren<SpawnerController>().ToList();
        if (_spawners == null)
        {
            return;
        }
        foreach (var spawner in _spawners)
        {
            spawner.InitializeSpawner();
        }
    }

    public void ActivateRoom(int currentDifficulty)
    {
        foreach (var spawner in _spawners)
        {
            spawner.ActivateSpawner(currentDifficulty);
        }

        if (_transitionRoomDisplay)
        {
            _transitionRoomDisplay.DisplayStatistics();
        }
    }

    private void ApplyDoors() 
    {
        foreach (var direction in _availableDirections)
        {
            GameObject spawnedWall = new GameObject();
            // Spawned Exit door
            if (direction == _exitDirection)
            {
                spawnedWall = Instantiate(GetWallObjectsFromDirection(direction)[1], transform.position, Quaternion.identity, transform);
                spawnedWall.transform.localPosition = GetWallPosFromDirectionAndType(direction, true);
                spawnedWall.GetComponentInChildren<CamUpTrigger>().Setup(direction);
            }
            // Spawns Normal wall
            else if (direction != _entranceDirection)
            {
                spawnedWall = Instantiate(GetWallObjectsFromDirection(direction)[0], transform.position, Quaternion.identity, transform);
                spawnedWall.transform.localPosition = GetWallPosFromDirectionAndType(direction);
            }
            
            if(direction == LevelController.Direction.Down) { spawnedWall.transform.rotation = Quaternion.Euler(0,0,180);}
            else if(direction == LevelController.Direction.Left) { spawnedWall.transform.rotation = Quaternion.Euler(0,180,0);}
        }
    }
    
    private Vector3 GetWallPosFromDirectionAndType(LevelController.Direction direction, bool typeIsDoor = false)
    {
        switch (direction)
        {
            case LevelController.Direction.Up:
                // TODO: i cant figure out why the hell the door prefab cant be positioned properly so this is here
                //return  new Vector2(0, (_roomSize.y / 2f));
                return typeIsDoor ? new Vector2(0, 8) : new Vector2(0, (_roomSize.y / 2f));
            case LevelController.Direction.Down:
                return new Vector2(0, -(_roomSize.y / 2f) );
            case LevelController.Direction.Left:
                return new Vector2(-(_roomSize.x / 2f), 0 );
            case LevelController.Direction.Right:
                return new Vector2(_roomSize.x / 2f, 0) ;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private List<GameObject> GetWallObjectsFromDirection(LevelController.Direction direction)
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

    public bool IsRoomDifficultEnough(int currentDifficulty)
    {
        return currentDifficulty >= _minimumDifficulty;
    }
    
    public bool IsRoomEligible(int currentDifficulty, LevelController.Direction entranceDirection, LevelController.Direction exitDirection)
    {
        return currentDifficulty >= _minimumDifficulty && Entrances.Directions.Contains(entranceDirection) 
                                                       && Exits.Directions.Contains(exitDirection);
    }

    public LevelController.Direction GetExitDirection()
    {
        return _exitDirection;
    }

    public List<LevelController.Direction> GetPossibleExitDirections()
    {
        List<LevelController.Direction> possibleDirections = Exits.Directions;
        
        // TODO: remove hacky solution to prevent remaking all prefabs using "all directions" holder for exits
        possibleDirections.Remove(LevelController.Direction.Down);
        
        return possibleDirections;
    }
    
    public Vector2 GetRoomSize()
    {
        return _roomSize;
    }
}
