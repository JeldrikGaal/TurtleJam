using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _health;
    [Range(0,100)]
    [SerializeField] private float _shieldSlowPercentage;
    [SerializeField] private GameObject _shieldObject;
    
    private ShieldScript _shieldLogic;
    private PlayerProjectile _playerProjectile;
    private Vector3 _shieldDirection;
    private bool _shieldReady;
    private bool _shieldFlying;
    
    private Rigidbody2D _rigidBody;

    private GameManager _gameManager;

    private bool _blockMovement;
    private bool _aiming;
    private bool _bounce;
    private bool _bouncing;
    private float _currentBounceDistance;
    
    private float _timeMod;
    private float _timeModSave;
    private Vector2 _reflectNormal;

    // Boomerang variables
    [SerializeField] private float _flyingTime;
    [SerializeField] private float _flyingTimeBack;
    private float _flyingSpeed;
    private Vector2 _startingPos;
    private Vector2 _startingPosBack;
    private Vector2 _endPos;
    private Vector2 _endPosSave;
    private float _startingTime;
    private float _startingTimeBack;
    private bool _flyingBack;

    // Needed for portals ?
    public bool teleporting = false;
    
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerProjectile = _shieldObject.GetComponent<PlayerProjectile>();
        _shieldReady = true;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // TODO: clean up pause mechanic
        if (_gameManager.paused)
        {
            _rigidBody.velocity = Vector2.zero;
            return;
        }
        Move();
        ShootInteraction();
        ShieldInteraction();
        
        // TODO: better movement blocking
        if (_blockMovement)
        {
            _rigidBody.velocity = Vector2.zero;
        }
    }
    
    private void ShootInteraction()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _playerProjectile.RequestShootProjectile();
        }
    }

    private void Move()
    {
        _rigidBody.velocity = new Vector2(_speed * Input.GetAxisRaw("Horizontal"), _speed * Input.GetAxisRaw("Vertical"));
        if (IsPlayerShielding())
        {
            _rigidBody.velocity *= 1 - ( _shieldSlowPercentage / 100);
        }
        
    }
    
    private void ShieldInteraction()
    {
        // Handling Right Click
        if (Input.GetMouseButtonDown(1))
        {
            _playerProjectile.RequestOpenShield();
        }       
        if (Input.GetMouseButtonUp(1))
        {
            _playerProjectile.RequestCloseShield();
        }
    }

    private bool IsPlayerShielding()
    {
        return _playerProjectile.IsShielding();
    }

    public void Damage(float dmg)
    {
        _health -= dmg;
        if (_health <= 0)
        {
            // TODO DIE SOUND
            _gameManager.GameOverCondition();
        }
    }
}
