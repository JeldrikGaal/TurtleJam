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
    [SerializeField] private float _shootRange;
    [SerializeField] private Vector3 _shieldShootScale;
    [SerializeField] private GameObject _shieldObject;
    [SerializeField] private float _shieldIdleDistanceToPlayer;
    private ShieldScript _shieldLogic;
    private Vector3 _shieldDirection;
    private bool _shieldReady;
    private bool _shieldFlying;
    
    private SpriteRenderer _shieldSpriteRenderer;
    private ParticleSystem _explosionParticle;
    
    private LineRenderer _lineRenderer;
    private Rigidbody2D _rigidBody;
    
    private CameraManager _cameraManager;
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

    private PlayAudio _playAudio;
    private Vector3 _shieldScaleSafe;

    // Needed for portals ?
    public bool teleporting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        
        _shieldSpriteRenderer = _shieldObject.GetComponent<SpriteRenderer>();
        _shieldLogic = _shieldObject.GetComponent<ShieldScript>();
        _shieldScaleSafe = _shieldObject.transform.localScale;
        _shieldReady = true;
        _playAudio = _shieldObject.GetComponent<PlayAudio>();
        
        _cameraManager = Camera.main.GetComponent<CameraManager>();
        
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _explosionParticle = _gameManager.explosion;
    }

    // Update is called once per frame
    void Update()
    {
        // Skip update loop if game is paused 
        // TODO: clean up pause mechanic
        if (_gameManager.paused)
        {
            _rigidBody.velocity = Vector2.zero;
            return;
        }

        PositionShield();
        Move();

        ShieldInteraction();
        ShootInteraction();
        
        if (_shieldFlying)
        {
            BommerangLogic();
        }
        if (_aiming)
        {
            Aiming();
        }

        // Needs to be at the end of update
        if (_blockMovement)
        {
            _rigidBody.velocity = Vector2.zero;
        }
    }

    private void ShootInteraction()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            _aiming = true;
            _lineRenderer.enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_shieldReady)
            {
                BoomerangShot();
               
            }
            _aiming = false;
            _lineRenderer.enabled = false;
        }
    }

    private void ShieldInteraction()
    {
        // Handling Right Click
        if (Input.GetMouseButtonDown(1))
        {
            if (_shieldReady)
            {
                _blockMovement = true;
                //ShieldSR.color = Color.red;
                _shieldLogic.ChangeState(2);
                _shieldReady = false;
                _playAudio.PlayOneShotSound(2);
            }
          
        }       
        if (Input.GetMouseButtonUp(1))
        {
            if (!_shieldReady)
            {
                _blockMovement = false;
                //ShieldSR.color = Color.green;
                _shieldLogic.ChangeState(0);
                _shieldReady = true;
            }

        }
    }

    private void Move()
    {
        _rigidBody.velocity = Vector3.zero;
        
        // TODO: improve very rudimentary input system 
        if (Input.GetKey(KeyCode.D))
        {
            _rigidBody.velocity = new Vector2(_speed, _rigidBody.velocity.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rigidBody.velocity = new Vector2(-_speed, _rigidBody.velocity.y);
        }
        if (Input.GetKey(KeyCode.W))
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, -_speed);
        }
    }

    private void PositionShield()
    {
        // Shield positioning 
        _shieldDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        _shieldDirection = new Vector3(_shieldDirection.x, _shieldDirection.y, 0);
        _shieldDirection.Normalize();
        _shieldObject.transform.position = transform.position + (_shieldDirection * _shieldIdleDistanceToPlayer);
        _shieldObject.transform.up  = _shieldDirection;

        transform.transform.up = _shieldDirection;
    }

    private void BommerangLogic()
    {
        // Move shield while flying
        if (Time.time - _startingTime < (_flyingTime * _timeMod))
        {
            _shieldObject.transform.position = Vector2.Lerp(_startingPos, _endPos, ( Time.time - _startingTime ) / (_flyingTime * _timeMod));
        }
        
        else if (!_flyingBack)
        {
            if (_bouncing)
            {
                _bouncing = false;
                _bounce = false;
            }

            if (_bounce && !_bouncing)
            {
                _timeMod = 1 - _timeModSave;
                _startingPos = _endPos;
                _endPos = Vector2.Reflect(_shieldDirection, _reflectNormal) * _currentBounceDistance;
                RaycastHit2D hit = Physics2D.Raycast(_startingPos, _reflectNormal);
                if (hit)
                {
                    if (Vector2.Distance(hit.point, _startingPos) <= _currentBounceDistance)
                    {
                        _endPos = hit.point;
                    }
                }
                
                _timeMod *= (Vector2.Distance(_startingPos, _endPos) / _currentBounceDistance);
                _startingTime = Time.time;
                _bouncing = true;
                
                Explosion(_startingPos);
                _playAudio.PlayOneShotSound(1);
            }
            else
            {
                _flyingBack = true;
                _startingTimeBack = Time.time;
                _shieldSpriteRenderer.color = new Color(_shieldSpriteRenderer.color.r, _shieldSpriteRenderer.color.g, _shieldSpriteRenderer.color.b, 0f);
                StartCoroutine(_cameraManager.Shake(0.05f, 0.2f));
                Explosion(_endPos);
                _playAudio.PlayOneShotSound(1);
            }
        }
        
        // Shield is flying back
        if (Time.time - _startingTimeBack < (_flyingTimeBack * _timeMod) && _flyingBack)
        {          
            _shieldObject.transform.position = Vector2.Lerp(_endPos, transform.position + (_shieldDirection * _shieldIdleDistanceToPlayer), (Time.time - _startingTimeBack) / (_flyingTimeBack * _timeMod));
        }
        
        // Shield has arrived back 
        if (_flyingBack && Time.time - _startingTimeBack > (_flyingTimeBack * _timeMod))
        {
            _shieldSpriteRenderer.color = new Color(_shieldSpriteRenderer.color.r, _shieldSpriteRenderer.color.g, _shieldSpriteRenderer.color.b, 1f);
            _flyingBack = false;
            _shieldFlying = false;
            _shieldReady = true;
            _shieldLogic.ChangeState(0);
            _shieldObject.transform.localScale = _shieldScaleSafe;
        }
    }

    private void Explosion(Vector2 pos)
    {
        ParticleSystem ps = Instantiate(_explosionParticle, pos, Quaternion.identity) as ParticleSystem;
        ps.Play();
        Destroy(ps.gameObject, 0.2f);
        StartCoroutine(_gameManager.flashWalls(0.05f, Color.red));
    }

    private void BoomerangShot()
    {
        _shieldObject.GetComponent<Animator>().SetTrigger("Shoot");
        _playAudio.PlayOneShotSound(0);
        
        _shieldObject.transform.localScale = _shieldShootScale;

        _shieldFlying = true;
        _shieldReady = false;
        
        _startingPos = _shieldObject.transform.position;
        _startingTime = Time.time;
        _endPos = transform.position + (_shieldDirection * _shootRange);
        _endPosSave = transform.position + (_shieldDirection * _shootRange);
        
        // TODO: rework shield state logic 
        _shieldLogic.ChangeState(1);

        Vector2 start = transform.position + (_shieldDirection);
        RaycastHit2D hit = Physics2D.Raycast(start, _shieldDirection);
        if (hit)
        {
            if (!hit.transform.CompareTag("Enemy"))
            {
                // ?? End the shot a bit behind the hit object --> not sure why tbh 
                _endPos = new Vector3(hit.point.x, hit.point.y) - (_shieldDirection * 0.25f); 
                
                if (hit.transform.CompareTag("Wall"))
                {
                    _bounce = true;
                    _reflectNormal = hit.normal;
                    _currentBounceDistance = _shootRange - Vector2.Distance(_startingPos, _endPos);
                }
            }
            else
            {
                _endPos = new Vector3(hit.point.x, hit.point.y) + (_shieldDirection * 0.25f);

            }
        
            _timeMod = Vector3.Distance(_endPos, _startingPos) / Vector3.Distance(_endPosSave, _startingPos);
            
            if (_bounce)
            {
                _timeModSave = _timeMod;
            }
        }
        else
        {
            _timeMod = 1;
        }
    }

    private void Aiming()
    {
        // TODO: find out if we still need this ?? 
        return;
        float len = 10f;
        Vector2 start = transform.position + (_shieldDirection * 0.5f);
        Vector2 end = transform.position + (_shieldDirection * 11);

        float dist = Vector2.Distance(start, end);
        len = dist;
        float dist2;
        RaycastHit2D hit = Physics2D.Raycast(start, _shieldDirection);
        if (hit && !hit.transform.CompareTag("Player") && !hit.transform.CompareTag("Shield"))
        {
            dist2 = Vector2.Distance(start, hit.transform.position);
            if (dist2 < dist)
            {
                end = hit.transform.position;
                len = dist2;
            }
        }
        
        Vector2 direction = end - start;
        direction.Normalize();

        for (int i = 0; i < 10; i++)
        {
                _lineRenderer.SetPosition(i, start + direction * (len * ((i + 1f) / 10)));
        }
    }

    public void Damage(float dmg)
    {
        if (_shieldLogic.shielding) return;
        _health -= dmg;
        if (_health <= 0)
        {
            // TODO DIE SOUND
            _gameManager.GameOverCondition();
        }
    }
}
