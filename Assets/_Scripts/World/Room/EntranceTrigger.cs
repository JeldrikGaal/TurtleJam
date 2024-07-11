using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class EntranceTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _doorBlock;
    [SerializeField] private GameObject _doorBlock2;
    [SerializeField] private List<GameObject> _doorTiles;
    [SerializeField] private List<EntranceTrigger> _partnerDoors;
    
    private LevelController.Direction _direction;
    
    List<Vector3> _doorTilePosititions = new List<Vector3>();

    private bool _closed;

    private bool _playerInEntrance;

    public static event Action<EntranceTrigger> RoomEntranced; 

    private void Awake()
    {
        ExitTrigger.roomExited += BlockGoingBack;
    }

    private void OnDestroy()
    {
        ExitTrigger.roomExited -= BlockGoingBack;
    }

    public void Setup(LevelController.Direction direction)
    {
        _direction = direction;
        
        foreach (var tile in _doorTiles)
        {
            _doorTilePosititions.Add(tile.transform.localPosition);
        }
    }
    
    private Vector3 GetEntranceDoorRotationFromDirection()
    {
        Vector3 startingRot = transform.rotation.eulerAngles;
        switch (_direction)
        {
            case LevelController.Direction.Left:
                return new Vector3(startingRot.x, startingRot.y, -90);
                
            case LevelController.Direction.Right:
                return new Vector3(startingRot.x, startingRot.y, 90);
        }

        return startingRot;
    }


    private void BlockGoingBack(ExitTrigger exitTrigger)
    {
        if (_playerInEntrance)
        {
            _doorBlock2.SetActive(true);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            _playerInEntrance = true;
            //_doorBlock2.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log((collision.transform.name, transform.name));
        if(collision.CompareTag("Player") && _playerInEntrance)
        {
            _playerInEntrance = false;
            if (!HasPlayerPassed(collision.transform.position)) return;
            RoomEntranced?.Invoke(this);
            BlockWall();
            _doorBlock2.SetActive(true);
        }
    }

    private bool HasPlayerPassed(Vector3 playerPos)
    {
        switch (_direction)
        {
            case LevelController.Direction.Up:
                return playerPos.y > transform.position.y;
            case LevelController.Direction.Down:
                return playerPos.y < transform.position.y;
            case LevelController.Direction.Left:
                return playerPos.x < transform.position.x;
            case LevelController.Direction.Right:
                return playerPos.x > transform.position.x;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void BlockWall()
    {
        if (_closed)
        {
            return;
        }

        _closed = true;
        _doorBlock.SetActive(true);
        DoorClosingEffect();
        CloseAllPartnerDoors();

    }

    private void CloseAllPartnerDoors()
    {
        
        foreach (var door in _partnerDoors)
        {
            door.Setup(LevelController.Direction.None);
            door.GetClosedByPartnerDoor();
        }
    }
    
    public void GetClosedByPartnerDoor()
    {
        BlockWall();
    }
    
    private void DoorClosingEffect()
    {
        PositionDoorTilesToClose();
        for (int i = 0; i < _doorTiles.Count - 1f; i++)
        {
            _doorTiles[i].SetActive(true);
            _doorTiles[i].transform.DOLocalMove(_doorTilePosititions[i], 0.5f);
        }

        _doorTiles[^1].SetActive(true);
        _doorTiles[^1].transform.DOLocalMove(_doorTilePosititions[^1], 0.5f).OnComplete(() =>
        {
            StartCoroutine(CameraManager.Instance.Shake(0.05f, 0.25f));
                
        });
        
        ColorsController.Instance.RegisterRangeToColorShift(transform.GetComponentsInChildren<SpriteRenderer>().ToList());
    }

    private void PositionDoorTilesToClose()
    {
        _doorTiles[0].transform.localPosition = new Vector3(_doorTiles[0].transform.localPosition.x - 1, _doorTiles[0].transform.localPosition.y, _doorTiles[0].transform.localPosition.z);
        _doorTiles[1].transform.localPosition = new Vector3(_doorTiles[1].transform.localPosition.x - 1, _doorTiles[1].transform.localPosition.y, _doorTiles[1].transform.localPosition.z);
        _doorTiles[2].transform.localPosition = new Vector3(_doorTiles[2].transform.localPosition.x + 1, _doorTiles[2].transform.localPosition.y, _doorTiles[2].transform.localPosition.z);
        _doorTiles[3].transform.localPosition = new Vector3(_doorTiles[3].transform.localPosition.x + 1, _doorTiles[3].transform.localPosition.y, _doorTiles[3].transform.localPosition.z);
    }
   
      
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, "ENTRANCE");
    }
    #endif
}
