using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelAttributes : MonoBehaviour
{
    [SerializeField] private int _minimumDifficulty;
    [FormerlySerializedAs("_entrace")] [SerializeField, EnumToggleButtons] private LevelController.Direction _entrance;
    [SerializeField, EnumToggleButtons] private LevelController.Direction _exit;
    private GameObject _entranceDoor;
    private GameObject _exitDoor;
    
    private List<SpawnerController> _spawners;
    private TransitionRoom _transitionRoom;
    

    [HideInInspector] public Vector2 EntranceOffset;

    private int _currentDifficulty;

    private void Awake()
    {
        _spawners = GetComponentsInChildren<SpawnerController>().ToList();
        
        if (TryGetComponent<TransitionRoom>(out var component))
        {
            _transitionRoom = component;
        }
        else
        {
            _transitionRoom = null;
        }

        _entranceDoor = GetComponentInChildren<EntranceTrigger>().gameObject;
        _exitDoor = GetComponentInChildren<ExitTrigger>().gameObject;

        EntranceOffset = transform.position - _entranceDoor.transform.position;
    }

    public void InitializeRoom()
    {
        InitializeSpawners();
        _exitDoor.GetComponent<ExitTrigger>().Setup(_exit);
        _entranceDoor.GetComponent<EntranceTrigger>().Setup( LevelController.GetEntranceDirectionFromExitDirection(_entrance));
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

        if (_transitionRoom)
        {
            _transitionRoom.DisplayStatistics();
        }

        _currentDifficulty = currentDifficulty;
    }

    public void SetupEntrance(LevelController.Direction entranceDirection)
    {
        _entrance = entranceDirection;
        _entranceDoor = _transitionRoom.GetDoor(entranceDirection);
        EntranceOffset = transform.position - _entranceDoor.transform.position;
        _entranceDoor.transform.rotation = Quaternion.Euler(GetEntranceDoorRotationFromDirection());
    }
    
    private Vector3 GetEntranceDoorRotationFromDirection()
    {
        Vector3 startingRot = _entranceDoor.transform.rotation.eulerAngles;
        switch (_entrance)
        {
            case LevelController.Direction.Left:
                return new Vector3(startingRot.x, startingRot.y, -90);
                
            case LevelController.Direction.Right:
                return new Vector3(startingRot.x, startingRot.y, 90);
        }

        return startingRot;
    }

     public LevelController.Direction GetEntranceDirection()
    {
        return _entrance;
    }
    
    public LevelController.Direction GetExitDirection()
    {
        return _exit;
    }

    public Vector2 GetExitDoorPos()
    {
        return _exitDoor.transform.position;
    }
    

    public bool IsLevelEligible(LevelController.Direction neededEntrance, int neededDifficulty)
    {
        return _entrance == neededEntrance && neededDifficulty >= _minimumDifficulty;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, gameObject.transform.name);
        Handles.Label(transform.position - new Vector3(0, 0.5f, 0), "Diff:" +_currentDifficulty.ToString());
    }
    #endif
    
}
