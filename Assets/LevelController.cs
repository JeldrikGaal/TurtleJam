using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<int> _progressionThreshold = new List<int>();

    [SerializeField] private List<GameObject> _stageTransitionRooms = new List<GameObject>();
    [SerializeField] private List<GameObject> _normalRooms = new List<GameObject>();
    
    [SerializeField] private int _currentStageIndex = 0;
    [SerializeField] private TextMeshProUGUI _stageNumUI;
    
    [SerializeField] private LevelAttributes _tutorialRoom;

    private Transform _gridTransform;
    
    private List<LevelAttributes> _generatedRooms = new List<LevelAttributes>();
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down,
        None
    }

    private void Start()
    {
        _gridTransform = GameObject.FindGameObjectWithTag("Grid").transform;
        _generatedRooms.Add(_tutorialRoom);
        
        GenerateBatch(_progressionThreshold[_currentStageIndex]);
       
    }

    private List<Direction> GeneratePath(int length)
    {
        List<Direction> path = new List<Direction>();
        path.Add(GetRandomDirection(GetAvailableDirections(Direction.Up)));
        
        for (int i = 1; i < length; i++)
        {
            path.Add(GetRandomDirection(GetAvailableDirections(path[i-1])));
        }

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
        //stageNumUI.text = (currentStageIndex + 1).ToString();

        GenerateBatch(_progressionThreshold[_currentStageIndex]);

        //stageTransitionRoom.GetComponentInChildren<CamUpTrigger>().UpdateCamUpTrigger();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ProgressToNextStage();
        }
    }


    /*
[Tooltip("Levels that their main entrance comes from the bottom")]
    public List<GameObject> levelsDown;
    [Tooltip("Levels that their main entrance comes from the top")]
    public List<GameObject> levelsUp;
    [Tooltip("Levels that their main entrance comes from the right")]
    public List<GameObject> levelsRight;
    [Tooltip("Levels that their main entrance comes from the left")]
    public List<GameObject> levelsLeft;

    public float yOffset;
    public float xOffset;

    private Transform grid;
    public List<Transform> generatedRooms;

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").transform;

        numOfRooms = progressionThreshold[currentStageIndex];

        GenerateLevel();
    }

    public void GenerateLevel()
    {
        for(int i = 0; i < numOfRooms; i++)
        {
            if (currentRoom.GetComponent<LevelAttributes>().hasExitUp &&
                !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have down && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsDown.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsDown[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsDown[j]);
                    else if (i != numOfRooms - 1 && !levelsDown[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsDown[j]);

                }

                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];

                if (i == numOfRooms - 1 && (currentStageIndex == 0 || currentStageIndex == 1 || currentStageIndex == 2))
                {
                    newRoomSelected = stageTransitionRooms[currentStageIndex];
                }


                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x, currentRoom.transform.localPosition.y + yOffset, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;
            }
            else if (currentRoom.GetComponent<LevelAttributes>().hasExitRight && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have left && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsLeft.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsLeft[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsLeft[j]);
                    else if (i != numOfRooms - 1 && !levelsLeft[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsLeft[j]);
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];

                if (i == numOfRooms - 1 && (currentStageIndex == 0 || currentStageIndex == 1 || currentStageIndex == 2))
                {
                    newRoomSelected = stageTransitionRooms[currentStageIndex];
                }

                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x + xOffset, currentRoom.transform.localPosition.y, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }
            else if (currentRoom.GetComponent<LevelAttributes>().hasExitLeft && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have right && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsRight.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsRight[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsRight[j]);
                    else if (i != numOfRooms - 1 && !levelsRight[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsRight[j]);
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];

                if (i == numOfRooms - 1 && (currentStageIndex == 0 || currentStageIndex == 1 || currentStageIndex == 2))
                {
                    newRoomSelected = stageTransitionRooms[currentStageIndex];
                }

                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x - xOffset, currentRoom.transform.localPosition.y, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }

            else if (currentRoom.GetComponent<LevelAttributes>().hasExitDown && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have up && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsUp.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsUp[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsUp[j]);
                    else if (i != numOfRooms - 1 && !levelsUp[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsUp[j]);
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];

                if (i == numOfRooms - 1 && (currentStageIndex == 0 || currentStageIndex == 1 || currentStageIndex == 2))
                {
                    newRoomSelected = stageTransitionRooms[currentStageIndex];
                }

                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x, currentRoom.transform.localPosition.y - yOffset, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }
        }
    }

    public void ProgressToNextStage()
    {
        currentStageIndex++;
        stageNumUI.text = (currentStageIndex + 1).ToString();

        GameObject stageTransitionRoom = currentRoom;

        numOfRooms = progressionThreshold[currentRoomIndex];
        GenerateLevel();

        stageTransitionRoom.GetComponentInChildren<CamUpTrigger>().UpdateCamUpTrigger();
    }
    */
}