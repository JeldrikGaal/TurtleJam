using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<int> _progressionThreshold = new List<int>();

    [SerializeField] private List<GameObject> _stageTransitionRooms = new List<GameObject>();
    [SerializeField] private List<GameObject> _normalRooms = new List<GameObject>();
    [SerializeField] private LevelAttributes _tutorialRoom;
    [SerializeField] private List<LevelAttributes> _generatedRooms = new List<LevelAttributes>();

    private int _currentStageIndex;
    private int _currentRoomIndex;

    private Transform _gridTransform;

    public static event Action TileMapsChanged;
    
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down,
        None
    }

    private void OnEnable()
    {
        CamUpTrigger.roomExited += NextRoom;
    }

    private void OnDestroy()
    {
        CamUpTrigger.roomExited -= NextRoom;
    }

    private void Start()
    {
        _gridTransform = GameObject.FindGameObjectWithTag("Grid").transform;
        _generatedRooms.Add(_tutorialRoom);
        
        GenerateBatch(_progressionThreshold[_currentStageIndex]);
    }

    private void Update()
    {
        // TODO: remove debugging
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ProgressToNextStage();
        }
    }

    private List<Direction> GeneratePath(int length)
    {
        // TODO: rethink if this is fine ? works so far
        if (length == 1)
        {
            return new List<Direction>() { Direction.Up };
        }

        List<Direction> possibleExitDirectionsForStage = GetAvailableExitDirectionsForDifficulty(_currentStageIndex);
        
        // Path needs to start with Room with connected room having an up exit
        var path = new List<Direction> 
        {
            GetRandomNextDirection(possibleExitDirectionsForStage, Direction.Up)
        };

        for (int i = 1; i < length-1; i++)
        {
            path.Add(GetRandomNextDirection(possibleExitDirectionsForStage, path[i-1]));
        }
        
        // Path needs to end with up direction
        path.Add(Direction.Up);

        return path;
    }
    
    private static Direction GetRandomNextDirection(List<Direction> availableDirectionFromRooms, Direction previousExit)
    {
        var availableDirections = availableDirectionFromRooms.ToList();
        availableDirections.Remove(previousExit);
        availableDirections.Remove(GetEntranceDirectionFromExitDirection(previousExit));
        return GetRandomDirectionFromList(availableDirections);
    }

    private static Direction GetEntranceDirectionFromExitDirection(Direction exitDirection)
    {
        switch (exitDirection)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
        }

        return Direction.None;
    }
    
    private static Direction GetRandomDirectionFromList(IReadOnlyList<Direction> directions)
    {
        return directions[Random.Range(0, directions.Count)];
    }

    private void GenerateBatch(int roomAmount)
    {
        List<LevelAttributes> rooms = new List<LevelAttributes>();
        List<Direction> path = GeneratePath(roomAmount);
        
        rooms.Add( GenerateRandomFromPreviousRoom(_tutorialRoom, path[0]));
        _generatedRooms.Add(rooms[0]);
        
        for (int i = 1; i < roomAmount; i++)
        {
            rooms.Add( GenerateRandomFromPreviousRoom(rooms[i - 1], path[i]));
        }
        
        rooms.Add(GenerateTransitionRoom(rooms[^1]));
        _tutorialRoom = rooms[^1];
        
        rooms.RemoveAt(0);
        
        _generatedRooms.AddRange(rooms);
        
        TileMapsChanged?.Invoke();
    }

    private LevelAttributes GenerateTransitionRoom(LevelAttributes previousRoom)
    {
        return GenerateRoom(previousRoom, Direction.Up, _stageTransitionRooms[_currentStageIndex]);
    }
    
    private LevelAttributes GenerateRandomFromPreviousRoom(LevelAttributes previousRoom, Direction exitDirection)
    {
        Direction entranceDirection = GetEntranceDirectionFromExitDirection(previousRoom.GetExitDirection());
        return GenerateRoom(previousRoom, exitDirection, 
            GetRandomRoomPrefab(_currentStageIndex, entranceDirection, exitDirection));
    }
    private LevelAttributes GenerateRoom(LevelAttributes previousRoom, Direction exitDirection, GameObject roomToGenerate )
    {
        Direction entranceDirection = GetEntranceDirectionFromExitDirection(previousRoom.GetExitDirection());
        var newRoom = Instantiate(roomToGenerate, _gridTransform);
        //newRoom.transform.SetParent(_gridTransform);
        newRoom.transform.position = GetNewPositionFromPreviousRoom(previousRoom);
        LevelAttributes newRoomAttributes = newRoom.GetComponent<LevelAttributes>();
        newRoomAttributes.InitializeRoom(entranceDirection, exitDirection, _currentStageIndex);

        return newRoomAttributes;
    }

    private Vector2 GetNewPositionFromPreviousRoom(LevelAttributes previousRoom)
    {
        Vector2 previousPosition = previousRoom.transform.position;

        float xOffset = 0;
        float yOffset = 0;

        switch (previousRoom.GetExitDirection())
        {
            //TODO: make this more modular and less rigid
            case Direction.Up:
                yOffset = previousRoom.GetRoomSize().y * 0.5f - 0.5f;
                break;
            case Direction.Left:
                xOffset = previousRoom.GetRoomSize().x * -0.5f - 2 ;
                break;
            case Direction.Right:
                xOffset = previousRoom.GetRoomSize().x * 0.5f + 2;
                break;
        }
        
        return new Vector2(previousPosition.x + xOffset, previousPosition.y + yOffset);
    }

    private GameObject GetRandomRoomPrefab(int difficulty, Direction entranceDirection, Direction exitDirection)
    {
        List<GameObject> availableRooms = GetAvailableRoomPrefabs(difficulty, entranceDirection, exitDirection);
        return availableRooms[Random.Range(0, availableRooms.Count)];
    }

    private List<Direction> GetAvailableExitDirectionsForDifficulty(int difficulty)
    {
        var availableDirections = new List<Direction>();
        foreach (var room in _normalRooms)
        {
            var roomAttributes = room.GetComponent<LevelAttributes>();
            if (!roomAttributes.IsRoomDifficultEnough(difficulty))
            {
                continue;
            }
            
            List<Direction> exitDirs = roomAttributes.GetPossibleExitDirections();
            foreach (var dir in exitDirs)
            {
                if (!availableDirections.Contains(dir))
                {
                    availableDirections.Add(dir);
                }
            }
        }
        
        return availableDirections;
    }
    
    private List<GameObject> GetAvailableRoomPrefabs(int difficulty, Direction entranceDirection, Direction exitDirection)
    {
        var availableRooms = new List<GameObject>();
        foreach (var room in _normalRooms)
        {
            if (room.GetComponent<LevelAttributes>().IsRoomEligible(difficulty, entranceDirection, exitDirection))
            {
                availableRooms.Add(room);
            }
        }

        return availableRooms;
    }
    
    public void ProgressToNextStage()
    {
        _currentStageIndex++;
        
        // Loop last stage infinitely
        if (_currentStageIndex >= _progressionThreshold.Count)
        {
            _currentStageIndex = _progressionThreshold.Count - 1;
        }
        
        GenerateBatch(_progressionThreshold[_currentStageIndex]);
    }

    private void NextRoom(CamUpTrigger camUpTrigger)
    {
        _currentRoomIndex++;
        camUpTrigger.InitiateTransition(_generatedRooms[_currentRoomIndex].transform.Find("CamPosition").position);
        _generatedRooms[_currentRoomIndex].ActivateRoom(_currentStageIndex);
    }
    
}