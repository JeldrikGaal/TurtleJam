using System;
using UnityEditor;
using UnityEngine;

public class EntranceTrigger : MonoBehaviour
{
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, "ENTRANCE");
    }
    #endif

    [SerializeField] private GameObject _doorBlock;

    private LevelController.Direction _direction;
    
    public void Setup(LevelController.Direction direction)
    {
        _direction = direction;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) 
        {
            if (!HasPlayerPassed(collision.transform.position)) return;
            BlockWall();
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
        _doorBlock.SetActive(true);
    }
}
