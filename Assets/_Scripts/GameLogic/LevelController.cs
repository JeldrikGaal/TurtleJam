using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<int> _progressionThreshold = new List<int>();

    [SerializeField] private List<GameObject> _stageTransitionRooms = new List<GameObject>();
    [SerializeField] private List<GameObject> _normalRooms = new List<GameObject>();
    
    private int _currentStageIndex = 0;
    private int _currentRoomIndex = 0;
    [SerializeField] private TextMeshProUGUI _stageNumUI;
    
    [SerializeField] private LevelAttributes _tutorialRoom;

    private Transform _gridTransform;
    
    [SerializeField] private List<LevelAttributes> _generatedRooms = new List<LevelAttributes>();
    private List<GameObject> _availableRooms = new List<GameObject>();
    private List<Direction> _availableDirections = new List<Direction>();
    private List<Direction> path = new List<Direction>();

    private GameManager _gameManager;
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
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _gridTransform = GameObject.FindGameObjectWithTag("Grid").transform;
        _generatedRooms.Add(_tutorialRoom);
        
        GenerateBatch(_progressionThreshold[_currentStageIndex]);
        _gameManager.UpdateTileMapList();
    }

    private void GeneratePath(int length)
    {
        path.Clear();
        // TODO: rethink if this is fine ? works flawlessly so far
        if (length == 1)
        {
            path.Add(Direction.Up);
        }
        GetAvailableExitDirectionsFromEntrance(Direction.Up);

        path.Add(GetRandomDirection(_availableDirections));
        
        for (int i = 1; i < length-1; i++)
        {
            GetAvailableExitDirectionsFromEntrance(path[i-1]);
            path.Add(GetRandomDirection(_availableDirections));
        }
        
        path.Add(Direction.Up);

    }
    
    public void GetAvailableExitDirectionsFromEntrance(Direction entranceDirection)
    {
        _availableDirections.Clear();
        
        switch (entranceDirection)
        {
            case Direction.Up:
                _availableDirections.Add(Direction.Left);
                _availableDirections.Add(Direction.Right);
                _availableDirections.Add(Direction.Up);
                break;
            case Direction.Left:
                _availableDirections.Add(Direction.Up);
                _availableDirections.Add(Direction.Left);
                break;
            case Direction.Right:
                _availableDirections.Add(Direction.Up);
                _availableDirections.Add(Direction.Right);
                break;
        }
    }

    public Direction GetEntranceDirectionFromExitDirection(Direction exitDirection)
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
    
    private Direction GetRandomDirection(List<Direction> directions)
    {
        return directions[Random.Range(0, directions.Count)];
    }

    private void GenerateBatch(int roomAmount)
    {
        List<LevelAttributes> rooms = new List<LevelAttributes>();
        GeneratePath(roomAmount);
        
        rooms.Add( GenerateRandomRoom(_tutorialRoom, path[0]));
        _generatedRooms.Add(rooms[0]);
        
        for (int i = 1; i < roomAmount; i++)
        {
            rooms.Add( GenerateRandomRoom(rooms[i - 1], path[i]));
        }
        
        rooms.Add(GenerateTransitionRoom(rooms[^1]));
        _tutorialRoom = rooms[^1];
        
        rooms.RemoveAt(0);
        
        _generatedRooms.AddRange(rooms);
    }

    private LevelAttributes GenerateTransitionRoom(LevelAttributes previousRoom)
    {
        return GenerateRoom(previousRoom, Direction.Up, _stageTransitionRooms[_currentStageIndex]);
    }
    
    private LevelAttributes GenerateRandomRoom(LevelAttributes previousRoom, Direction exitDirection)
    {
        Direction entranceDirection = GetEntranceDirectionFromExitDirection(previousRoom.GetExitDirection());
        return GenerateRoom(previousRoom, exitDirection, GetRandomRoomPrefab(_currentStageIndex, entranceDirection, exitDirection));
    }
    private LevelAttributes GenerateRoom(LevelAttributes previousRoom, Direction exitDirection, GameObject roomToGenerate )
    {
        Direction entranceDirection = GetEntranceDirectionFromExitDirection(previousRoom.GetExitDirection());
        var newRoom = Instantiate(roomToGenerate, _gridTransform);
        newRoom.transform.SetParent(_gridTransform);
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
        
        GetAvailableRoomPrefabs(difficulty, entranceDirection, exitDirection);
        return _availableRooms[Random.Range(0, _availableRooms.Count)];
    }

    private void GetAvailableRoomPrefabs(int difficulty, Direction entranceDirection, Direction exitDirection)
    {
        _availableRooms.Clear();

        foreach (var room in _normalRooms)
        {
            if (room.GetComponent<LevelAttributes>().IsRoomEligible(difficulty, entranceDirection, exitDirection))
            {
                _availableRooms.Add(room);
            }
        }

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
        
        _gameManager.UpdateTileMapList();
        
        //stageNumUI.text = (currentStageIndex + 1).ToString();
    }

    private void NextRoom(CamUpTrigger camUpTrigger)
    {
        _currentRoomIndex++;
        camUpTrigger.InitiateTransition(_generatedRooms[_currentRoomIndex].transform.Find("CamPosition").position);
        _generatedRooms[_currentRoomIndex].ActivateRoom(_currentStageIndex);
    }
    
}