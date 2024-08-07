using System;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    
    public static event Action<ExitTrigger> roomExited; 
    public GameObject _camera;

    public Vector3 _nextPos; // Camera next (room's) position.
    
    [SerializeField] private float duration = 1f; // Duration of camera transition (lerp)

    [SerializeField] private GameObject _doorGlow;

    private LevelController.Direction _direction;

    private bool _exitedOnce;
    
    void Start()
    {
        _camera = Camera.main.gameObject;
        if (_camera == null)
        {
            Debug.LogError("Camera is null");
        }
    }
    
    void Update() 
    {
        if (Vector3.Distance(_camera.transform.position, _nextPos) <= 0.2f)
        {
            ColorsController.Instance.InformAboutObjectDestruction(gameObject);
            Destroy(this.gameObject);
        }
    }

    public void Setup(LevelController.Direction direction)
    {
        _direction = direction;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) 
        {
            if (!HasPlayerPassed(collision.transform.position) || _exitedOnce) return;
            roomExited?.Invoke(this);
            _exitedOnce = true;
            _doorGlow.GetComponent<SpriteRenderer>().enabled = false;
            
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
    
    public void InitiateTransition(Vector3 nextPos)
    {
        _nextPos = nextPos;
        _camera.GetComponent<CameraManager>().CameraTransition(_nextPos,duration);
    }
}
