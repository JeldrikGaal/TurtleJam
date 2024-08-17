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
    private GameObject _lastGeneratedRoomPrefab;

    private int _currentStageIndex;
    private int _currentLevelIndex;
    public int _totalLevelIndex = 1;

    private Transform _gridTransform;
    public static event Action<List<GameObject>> TileMapsChanged;
    public static event Action<int> ProgressedToNextStage;
    public static LevelController Instance;
    
    private LevelAttributes _currentLevel;
    private LevelAttributes _nextLevel;

    private int _enemiesFoundInStage;
    private int _enemiesKilledAtBeginningOfStage;

    private bool _stageCleared;
    
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down,
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
            _lastGeneratedRoomPrefab = roomToGenerate;
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

        // If there is more than one option available remove all occurrences of the last used room
        if (possibleRooms.Where(x => _lastGeneratedRoomPrefab).ToList().Count > 0)
        {
            possibleRooms.RemoveAll(o => o == _lastGeneratedRoomPrefab);
        }

        return possibleRooms;
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

        _enemiesKilledAtBeginningOfStage = StatisticManager.Instance.GetStatistics().EnemiesKilled;
        _enemiesFoundInStage = 0;
        
        // Loop last stage infinitely
        if (_currentStageIndex >= _progressionThreshold.Count)
        {
            _currentStageIndex = _progressionThreshold.Count - 1;
        }

        ProgressedToNextStage?.Invoke(_currentStageIndex);
    }
    
    private void NextRoom(ExitTrigger exitTrigger)
    {
        _totalLevelIndex++;
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

    public bool IsNextRoomTransition()
    {
        return _nextLevel.IsTransitionRoom();
    }

    public void SetStageCleared(bool newValue)
    {
        _stageCleared = newValue;
    }

    public void RegisterEnemyCount(int enemyAmountToRegister)
    {
        _enemiesFoundInStage += enemyAmountToRegister;
    }

    private string GetStageClearedText()
    {
        int totalEnemyKills = StatisticManager.Instance.GetStatistics().EnemiesKilled;
        int totalStageEnemyKills = totalEnemyKills - _enemiesKilledAtBeginningOfStage;
        if (totalStageEnemyKills >= _enemiesFoundInStage)
        {
            return "Enemies Killed: " + totalStageEnemyKills + "/" + _enemiesFoundInStage;
        }
        else
        {
            return "Enemies Killed: " + totalStageEnemyKills + "/" + _enemiesFoundInStage;
        }
    }
    
    public void RequestShowStageCleared()
    {
        PopupProvider.Instance.ShowPopupFromText(GetStageClearedText());
    }
    
}