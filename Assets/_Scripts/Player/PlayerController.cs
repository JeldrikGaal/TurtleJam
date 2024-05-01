using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
   
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _baseHealth;
    private float _speed;
    private float _health;
    
    [Range(0,100)]
    [SerializeField] private float _shieldSlowPercentage;
    [SerializeField] private GameObject _shieldObject;
    
    [SerializeField] private TMP_Text _tutorialInfoTextField;
    [SerializeField] private SpriteRenderer _bubbleShieldVisuals;
    public static event Action<int> NewBubbleShieldValueJustDropped;
    
    private ShieldScript _shieldLogic;
    private PlayerProjectile _playerProjectile;
    private Vector3 _shieldDirection;
    private bool _shieldReady;
    private bool _shieldFlying;
    
    private int _currentBubbleShieldAmount;
    
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

    // Needed for portals ?
    public bool teleporting = false;
    

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _playerProjectile = _shieldObject.GetComponent<PlayerProjectile>();
        _shieldReady = true;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        _speed = _baseSpeed;
        _health = _baseHealth;
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
        // TODO: Decide if we will need float dmg or int dmg
        _health -= FilterDamageThroughBubbleShield((int)dmg);
        if (_health <= 0)
        {
            // TODO DIE SOUND
            _gameManager.GameOverCondition();
        }
    }

    public void RequestSpeedChange(float speedDataSpeedIncrease)
    {
        _speed += speedDataSpeedIncrease;
    }

    public void RequestSpeedReset()
    {
        _speed = _baseSpeed;
    }

    public void RequestRangeChange(float decreaseDataRangeDecrease)
    {
        _playerProjectile.RequestRangeChange(decreaseDataRangeDecrease);
    }

    public void RequestRangeReset()
    {
        _playerProjectile.RequestRangeReset();
    }

    public void RequestShieldAdd(int shieldDataBubbleAmount)
    {
        _currentBubbleShieldAmount = shieldDataBubbleAmount;
        RefreshBubbleVisuals();
    }

    public void RequestShieldReset()
    {
        _currentBubbleShieldAmount = 0;
        RefreshBubbleVisuals();
    }
    

    private int FilterDamageThroughBubbleShield(int dmg)
    {
        if (_currentBubbleShieldAmount > 0)
        {
            _currentBubbleShieldAmount--;
            RefreshBubbleVisuals();
            SendBubbleChangedEvent(_currentBubbleShieldAmount);
            return dmg - 1;
        }
       
        return dmg;
    }

    private void SendBubbleChangedEvent(int bubbleAmount)
    {
        NewBubbleShieldValueJustDropped?.Invoke(bubbleAmount);
    }

    private void RefreshBubbleVisuals()
    {
        if (_currentBubbleShieldAmount > 0)
        {
            ActivateBubbleShieldVisuals();
        }
        else
        {
            DeactivateBubbleShieldVisuals();
            
        }
    }

    private void DeactivateBubbleShieldVisuals()
    {
        _bubbleShieldVisuals.enabled = false;
    }

    private void ActivateBubbleShieldVisuals()
    {
        _bubbleShieldVisuals.enabled = true;
    }
}
