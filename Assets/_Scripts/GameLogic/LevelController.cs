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
    private int _currentLevelIndex;

    private Transform _gridTransform;
    public static event Action<List<GameObject>> TileMapsChanged;

    private LevelAttributes _currentLevel;
    private LevelAttributes _nextLevel;
    
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
        ExitTrigger.roomExited += NextRoom;
    }

    private void OnDestroy()
    {
        ExitTrigger.roomExited -= NextRoom;
    }
    
    private void Start()
    {
        _gridTransform = GameObject.FindWithTag("Grid").transform;
        
        _currentLevel = _tutorialRoom;
        _nextLevel = CreateNextRoom();
    }

    private LevelAttributes CreateNextRoom()
    {
        Direction neededEntranceDirection = GetEntranceDirectionFromExitDirection(_currentLevel.GetExitDirection());
        LevelAttributes newRoom = null;
        if (_currentLevelIndex == _progressionThreshold[_currentStageIndex] - 1)
        {
            GameObject roomToGenerate = _stageTransitionRooms[_currentStageIndex];
            newRoom = GenerateRoom(roomToGenerate);
            newRoom.SetupEntrance( GetEntranceDirectionFromExitDirection(_currentLevel.GetExitDirection()));
            _currentLevelIndex = -1;
        }
        else
        {
            GameObject roomToGenerate = GetRandomPossibleRoom(neededEntranceDirection, _currentStageIndex);
            newRoom = GenerateRoom(roomToGenerate);
        }
        
        newRoom.InitializeRoom();
        PositionRoom(newRoom, _currentLevel);
        return newRoom;
    }
    
    private void PositionRoom(LevelAttributes roomToPosition, LevelAttributes roomToConnect)
    {
        Vector2 offset = roomToPosition.EntranceOffset;

        switch (roomToPosition.GetEntranceDirection())
        {
            case Direction.Up:
                offset.y -= 0.5f;
                break;
            case Direction.Left:
                offset.x += 0.5f;
                break;
            case Direction.Right:
                offset.x -= 0.5f;
                break;
            case Direction.Down:
                offset.y += 0.5f;
                break;
        }
        
        roomToPosition.transform.position = roomToConnect.GetExitDoorPos() + offset;
        
    }

    private GameObject GetRandomPossibleRoom(Direction entranceDirection, int currentDifficulty)
    {
        List<GameObject> roomOptions = GetPossibleRooms(entranceDirection, currentDifficulty);
        if (roomOptions.Count == 0)
        {
            Debug.LogError("No suitable rooms found ! ");
        }
        return roomOptions[Random.Range(0, roomOptions.Count)];
    }
    
    private List<GameObject> GetPossibleRooms(Direction entranceDirection, int currentDifficulty)
    {
        var possibleRooms = new List<GameObject>();
        foreach (var room in _normalRooms)
        {
            LevelAttributes roomAttributes = room.GetComponent<LevelAttributes>();
            if (roomAttributes.IsLevelEligible(entranceDirection, currentDifficulty))
            {
                possibleRooms.Add(room);
            }
        }

        return possibleRooms;
    }
    
    private void Update()
    {
        // TODO: remove debugging
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ProgressToNextStage();
        }
    }
    
    public static Direction GetEntranceDirectionFromExitDirection(Direction exitDirection)
    {
        switch (exitDirection)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Down:
                return Direction.Up;
        }

        return Direction.None;
    }
    
    private LevelAttributes GenerateRoom(GameObject roomToGenerate )
    {
        var newRoom = Instantiate(roomToGenerate, _gridTransform);
        LevelAttributes newRoomAttributes = newRoom.GetComponent<LevelAttributes>();
        return newRoomAttributes;
    }
    
    public void ProgressToNextStage()
    {
        _currentStageIndex++;
        
        // Loop last stage infinitely
        if (_currentStageIndex >= _progressionThreshold.Count)
        {
            _currentStageIndex = _progressionThreshold.Count - 1;
        }
    }
    
    private void NextRoom(ExitTrigger exitTrigger)
    {
        _currentLevelIndex++;
        LevelAttributes oldRoom = _currentLevel;
        
        _currentLevel = _nextLevel;
        
        exitTrigger.InitiateTransition(_currentLevel.transform.Find("CamPosition").position);
        _nextLevel = CreateNextRoom();
        _currentLevel.ActivateRoom(_currentStageIndex);

        
        
        Destroy(oldRoom.gameObject, 0.1f);
        
        var rooms = new List<GameObject>{ _currentLevel.gameObject, _nextLevel.gameObject};
        TileMapsChanged?.Invoke(rooms);
    }
    
}