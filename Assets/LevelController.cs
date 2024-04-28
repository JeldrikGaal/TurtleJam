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
    
    [SerializeField] private int _currentStageIndex = 0;
    private int _currentRoomIndex = 0;
    [SerializeField] private TextMeshProUGUI _stageNumUI;
    
    [SerializeField] private LevelAttributes _tutorialRoom;

    private Transform _gridTransform;
    
    [SerializeField] private List<LevelAttributes> _generatedRooms = new List<LevelAttributes>();
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

    private List<Direction> GeneratePath(int length)
    {
        // TODO: temp fix
        if (length == 1)
        {
            return new List<Direction>() { Direction.Up };
        }
        List<Direction> path = new List<Direction>();
        path.Add(GetRandomDirection(GetAvailableDirections(Direction.Up)));
        
        for (int i = 1; i < length-1; i++)
        {
            Debug.Log("111");
            path.Add(GetRandomDirection(GetAvailableDirections(path[i-1])));
        }
        
        path.Add(Direction.Up);

        return path;
    }
    
    public List<Direction> GetAvailableDirections(Direction entranceDirection)
    {
        List<Direction> availableDirections = new List<Direction>();
        switch (entranceDirection)
        {
            case Direction.Up:
                availableDirections.Add(Direction.Left);
                availableDirections.Add(Direction.Right);
                availableDirections.Add(Direction.Up);
                break;
            case Direction.Left:
                availableDirections.Add(Direction.Up);
                availableDirections.Add(Direction.Left);
                break;
            case Direction.Right:
                availableDirections.Add(Direction.Up);
                availableDirections.Add(Direction.Right);
                break;
        }
        return availableDirections;
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
        List<Direction> path = GeneratePath(roomAmount);
        
        rooms.Add( GenerateRandomRoom(_tutorialRoom, path[0]));
        _generatedRooms.Add(rooms[0]);
        
        for (int i = 1; i < roomAmount; i++)
        {
            Debug.Log(path[i]);
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
        return GenerateRoom(previousRoom, exitDirection, GetRandomRoomPrefab(_currentStageIndex, entranceDirection));
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
            case Direction.Up:
                yOffset = previousRoom.GetRoomSize().y * 0.5f;
                break;
            case Direction.Left:
                xOffset = previousRoom.GetRoomSize().x * -0.5f;
                break;
            case Direction.Right:
                xOffset = previousRoom.GetRoomSize().x * 0.5f;
                break;
        }
        
        return new Vector2(previousPosition.x + xOffset, previousPosition.y + yOffset);
    }

    private GameObject GetRandomRoomPrefab(int difficulty, Direction entranceDirection)
    {
        Debug.Log(entranceDirection);
        List<GameObject> availableRooms = GetAvailableRoomPrefabs(difficulty, entranceDirection);
        return availableRooms[Random.Range(0, availableRooms.Count)];
    }

    private List<GameObject> GetAvailableRoomPrefabs(int difficulty, Direction entranceDirection)
    {
        List<GameObject> availableRooms = new List<GameObject>();
        foreach (var room in _normalRooms)
        {
            if (room.GetComponent<LevelAttributes>().IsRoomEligible(difficulty, entranceDirection))
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
        
        //stageNumUI.text = (currentStageIndex + 1).ToString();
    }

    private void NextRoom(CamUpTrigger camUpTrigger)
    {
        _currentRoomIndex++;
        camUpTrigger.InitiateTransition(_generatedRooms[_currentRoomIndex].transform.Find("CamPosition").position);
        _generatedRooms[_currentRoomIndex].ActivateSpawners(_currentStageIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ProgressToNextStage();
        }
    }
    
}