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
    private EntranceTrigger _entranceTrigger;
    private GameObject _exitDoor;
    private ExitTrigger _exitTrigger; 
    
    private List<SpawnerController> _spawners;
    private TransitionRoom _transitionRoom;

    [HideInInspector] public Vector2 EntranceOffset;

    private int _currentDifficulty;
    private int _initialEnemyCount;
    private bool _roomActive;

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

        _entranceTrigger = GetComponentInChildren<EntranceTrigger>();
        _entranceDoor = _entranceTrigger.gameObject;
        
        _exitTrigger = GetComponentInChildren<ExitTrigger>();
        _exitDoor = _exitTrigger.gameObject;

        EntranceOffset = transform.position - _entranceDoor.transform.position;

        ExitTrigger.roomExited += ReactToRoomExited;
        EntranceTrigger.RoomEntranced += ReactToRoomEntered;
        EnemyController.EnemyDeath += ReactToEnemyDeathWithDelay;
    }

    private void OnDestroy()
    {
        ExitTrigger.roomExited -= ReactToRoomExited;
        EntranceTrigger.RoomEntranced -= ReactToRoomEntered;
        EnemyController.EnemyDeath -= ReactToEnemyDeathWithDelay;
        
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

    private int CountActiveEnemies()
    {
        int count = 0;
        foreach (var controller in transform.GetComponentsInChildren<EnemyController>())
        {
            if (controller.gameObject.activeInHierarchy)
            {
                count++;
            }
        }

        return count;
    }

    private void ReactToRoomExited(ExitTrigger exitTrigger)
    {
        if (_exitTrigger == exitTrigger)
        {
            _roomActive = false;
        }
    }
    
    private void ReactToRoomEntered(EntranceTrigger entranceTrigger)
    {
        if (!IsTransitionRoom())
        {
            return;
        }
        Debug.Log(_entranceTrigger.transform.GetInstanceID() + " " + entranceTrigger.transform.GetInstanceID() + " " + IsTransitionRoom());
        if (_entranceTrigger == entranceTrigger && IsTransitionRoom())
        {
            Debug.Log("REQUESTED");
            LevelController.Instance.RequestShowStageCleared();
        }
    }

    private void ReactToEnemyDeathWithDelay()
    {
       Invoke(nameof(ReactToEnemyDeath), 0.05f);
    }

    private void ReactToEnemyDeath()
    {
        if (CountActiveEnemies() == 0 && _roomActive)
        {
            if (LevelController.Instance.IsNextRoomTransition())
            {
                //PopupProvider.Instance.ShowPopup("StageClear");
                LevelController.Instance.SetStageCleared(true);
            }
            else
            {
                //PopupProvider.Instance.ShowPopup("RoomClear");
            }
            
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
        
        _initialEnemyCount = CountActiveEnemies();
        LevelController.Instance.RegisterEnemyCount(_initialEnemyCount);
        _roomActive = true;
    }

    public void SetupEntrance(LevelController.Direction entranceDirection)
    {
        _entrance = entranceDirection;
        _entranceDoor = _transitionRoom.GetDoor(entranceDirection);
        _entranceTrigger = _entranceDoor.GetComponent<EntranceTrigger>();
        EntranceOffset = transform.position - _entranceDoor.transform.position;
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

    public bool IsTransitionRoom()
    {
        return _transitionRoom;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, gameObject.transform.name);
        Handles.Label(transform.position - new Vector3(0, 0.5f, 0), "Diff:" +_currentDifficulty.ToString());
    }
    #endif
    
}
