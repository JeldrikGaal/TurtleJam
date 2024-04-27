using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamUpTrigger : MonoBehaviour
{
    private LevelController _levelController;
    public GameObject _camera;

    private Vector3 _initialPos; // Current camera position.
    public Vector3 _nextPos; // Camera next (room's) position.
    
    [SerializeField] private float duration = 1f; // Duration of camera transition (lerp)
    private float _startingTime = 0; // Reference for duration calc.
    
    private bool _movingFlag = false; // Is the camera currently moving?
    private bool _doneMoving = false; // Is the camera done moving to next position?


    // Start handles required variable assignments
    void Start()
    {
        _camera = Camera.main.gameObject;
        if (_camera == null)
        {
            Debug.LogError("Camera is null");
        }

        _initialPos = _camera.transform.position;
        _levelController = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelController>();
        if (transform.parent.GetComponent<LevelAttributes>().nextRoomConnected != null)
            _nextPos = transform.parent.GetComponent<LevelAttributes>().nextRoomConnected.transform.Find("CamPosition")
                .position;
    }

    // Update handles camera movement / arrival actions.
    void Update() 
    {
        if (_camera != null)
        {
            if (!_doneMoving && _movingFlag && _camera.transform.position != _nextPos)
            {
                _camera.transform.position = Vector3.Lerp(_initialPos, _nextPos, Time.time - _startingTime / duration);
            }
            else if (_doneMoving == false && _camera.transform.position == _nextPos)
            {
                _doneMoving = true;
                _movingFlag = false;
            }
        }
        else
        {
            Debug.Log("Camera doesn't exit bitch!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !_movingFlag) 
        {
            if (_camera == null)
                _camera = GameObject.FindGameObjectWithTag("MainCamera");
            InitiateTransition();
        }
    }

    private void InitiateTransition()
    {
        _initialPos = _camera.transform.position;
        _movingFlag = true;
        _startingTime = Time.time; 
    }
}
