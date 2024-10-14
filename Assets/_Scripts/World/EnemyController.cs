using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{

    public enum EnemyType
    {
        Stationary,
        Melee,
        Patrol,
        Chaser,
        Sticky
    }

    public EnemyType enemyMode;
    public float speed = 10f;
    public float rotationSpeed = 10f;
 


    [Space(20)]
    public Transform[] patrolTransforms;
    public Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isPatrollingForward = true;


    [Space(10)]
    public bool lookAtPlayer = true; // To indicate if the enemy should look at player; also for future to indicate if player is in enemy range.


    public float bulletTimeIntervals = 1f; // Seconds to wait between each enemy bullet shot.
    private bool shootTimedown = false;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public int _scoreToAddOnDeath = 10;

    private float time;
    private Transform player;
    private GameManager gm;

    private Rigidbody2D rb;
    public Animator _animator;
    private bool _isSoundPlaying = false;
    private bool _isActivated = false;

    public float detectionRadius = 10f;
    public LayerMask obstacleLayer;



    public static event Action EnemyDeath;
    public static event Action<Vector3> EnemyDeathWithLocation;

    private bool _isPlayerStuck;
    private BulletController bulletController;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        InitializePatrolPoints();
        //_animator = GetComponent<Animator>();   
    }
    private void InitializePatrolPoints(){
        if(patrolTransforms.Length>0)
        {
            patrolPoints = new Vector3[patrolTransforms.Length];
            
            for(int i = 0; i< patrolTransforms.Length; i++)
            {
                patrolPoints[i] = patrolTransforms[i].position;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (! GameStateManager.Instance.IsRunning())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Calculate the direction towards the player
        Vector2 directionToPlayer = player.position - transform.position;

        // Perform a raycast to check for obstacles between the enemy and the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, obstacleLayer);

        // Check if the raycast hit the player
        if (hit.collider != null)
        {
            if (hit.collider.gameObject == player.gameObject || hit.transform.CompareTag("Shield"))
            {
                lookAtPlayer = true;
            }
            else
            {
                lookAtPlayer = false;
            }
        }


        switch (enemyMode)
        {
            case EnemyType.Melee when lookAtPlayer:
            {
                Vector3 direction = ( player.position - transform.position).normalized;
                rb.velocity = direction * speed;
                if(!_isSoundPlaying){
                    SoundManager.PlaySound(SoundManager.Sound.EnemyMove, this.transform, true, SoundManager.SoundType.SFX);
                    _isSoundPlaying = true;
                }

                break;
            }
            case EnemyType.Chaser when _isActivated:
            {
                StartCoroutine(ActivateChaser(1f));
                break;
            }
            case EnemyType.Melee:
            {
                rb.velocity = Vector3.zero;
                if(_isSoundPlaying){
                SoundManager.StopSound(SoundManager.Sound.EnemyMove, this.transform);
                _isSoundPlaying = false;
                }

                break;
            }
            case EnemyType.Stationary:
            {
                Vector3 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Rotate the object around the Z-axis
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                time += Time.deltaTime;
                if (!shootTimedown)
                {
                    BasicShoot();
                }

                if (time > bulletTimeIntervals)
                {
                    time = 0;
                    shootTimedown = false;
                }
                break;
            }
            case EnemyType.Sticky when _isPlayerStuck:
            {
                Vector3 direction = ( player.position - transform.position).normalized;
                rb.velocity = direction * speed;
                break;
            }
/*
            case EnemyType.Sticky when lookAtPlayer:
            {
                Vector3 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Rotate the object around the Z-axis
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                time += Time.deltaTime;
                if (!shootTimedown && !_isPlayerStuck)
                {
                    StickShoot();
                }

                if (time > bulletTimeIntervals)
                {
                    time = 0;
                    shootTimedown = false;
                }
                break;
            }
*/
            case EnemyType.Sticky:
            {
                if(bulletController!=null){

                        _isPlayerStuck = bulletController.StickHit;
                }
                if(lookAtPlayer)
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Rotate the object around the Z-axis
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                time += Time.deltaTime;
                if (!shootTimedown && !_isPlayerStuck)
                {
                    StickShoot();
                }

                if (time > bulletTimeIntervals)
                {
                    time = 0;
                    shootTimedown = false;
                }
                }
                break;

            }

            case EnemyType.Chaser:
            {
                rb.velocity = Vector3.zero;
                
                
                _animator.Play("Inactive");

                break;
            }
            /*
            case EnemyType.Patrol:
            {
                Vector3 targetWaypoint = patrolPoints[currentPatrolIndex];
                Vector3 direction = ( targetWaypoint - transform.position).normalized;
                rb.velocity = direction * speed;
                    
                    if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
                    {
                        currentPatrolIndex++;
                        if (currentPatrolIndex >= patrolPoints.Length)
                          currentPatrolIndex = 0;
                    }
                //_animator.Play("Inactive");

                break;
            }
            */
        }
    }

    public void BasicShoot() 
    {       
        SoundManager.PlayRandomOneShot(SoundManager.Sound.EnemyShoot);
        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        shootTimedown = true; // Flag to indicate there should be timedown between each shot.
    }
    public void StickShoot() 
    {       
        SoundManager.PlayRandomOneShot(SoundManager.Sound.EnemyShoot);
        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        shootTimedown = true; // Flag to indicate there should be timedown between each shot.
        bulletController = newBullet.GetComponent<BulletController>();
    }
    

    public void Die() 
    {
        EnemyDeath?.Invoke();
        EnemyDeathWithLocation?.Invoke(transform.position);
        if(_isSoundPlaying)
        {
                SoundManager.StopSound(SoundManager.Sound.EnemyMove, this.transform);
                _isSoundPlaying = false;
        }

        SoundManager.PlayOneShotSound(SoundManager.Sound.EnemyDeath);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) 
        {
            collision.gameObject.GetComponent<PlayerController>().Damage(5, gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Player")){
            _isActivated = true;
            
        }
    }

    private IEnumerator ActivateChaser(float delay){
        _animator.Play("Activating");
        yield return new WaitForSeconds(delay);
         _animator.Play("Activated");
         Collider2D collider = GetComponent<Collider2D>();   
         collider.isTrigger = false; 
                Vector3 direction = ( player.position - transform.position).normalized;
                rb.velocity = direction * speed;
                if(!_isSoundPlaying){
                    SoundManager.PlaySound(SoundManager.Sound.EnemyMove, this.transform, true, SoundManager.SoundType.SFX);
                    _isSoundPlaying = true;
                }
    }
        private IEnumerator ActivateBoss(float delay){
        _animator.Play("Activating");
        yield return new WaitForSeconds(delay);
         _animator.Play("Activated");
         Collider2D collider = GetComponent<Collider2D>();   
         collider.isTrigger = false; 
                Vector3 direction = ( player.position - transform.position).normalized;
                rb.velocity = direction * speed;
                if(!_isSoundPlaying){
                    SoundManager.PlaySound(SoundManager.Sound.EnemyMove, this.transform, true, SoundManager.SoundType.SFX);
                    _isSoundPlaying = true;
                }
    }


}
