using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamUpTrigger : MonoBehaviour
{
    
    public static event Action<CamUpTrigger> roomExited; 
    public GameObject _camera;

    public Vector3 _nextPos; // Camera next (room's) position.
    
    [SerializeField] private float duration = 1f; // Duration of camera transition (lerp)

    [SerializeField] private GameObject _doorGlow;
    [SerializeField] private GameObject _doorBlock;
    
    private bool _movingFlag = false; // Is the camera currently moving?
    private bool _doneMoving = false; // Is the camera done moving to next position?

    
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
            Destroy(this.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !_movingFlag) 
        {
            if (_camera == null)
                _camera = GameObject.FindGameObjectWithTag("MainCamera");
            roomExited?.Invoke(this);
            BlockWall();
        }
    }

    private void BlockWall()
    {
        _doorBlock.SetActive(true);
        _doorGlow.GetComponent<SpriteRenderer>().enabled = false;
    }
    
    public void InitiateTransition(Vector3 nextPos)
    {
        _nextPos = nextPos;
        _movingFlag = true;
        _camera.GetComponent<CameraManager>().CameraTransition(_nextPos,duration);
    }
}
