using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class EnemyController : MonoBehaviour
{

    public enum EnemyType
    {
        Stationary,
        Melee,
        Patrol
    }

    public EnemyType enemyMode;
    public float speed = 10f;


    [Space(20)]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isPatrollingForward = true;


    [Space(10)]
    public bool lookAtPlayer = true; // To indicate if the enemy should look at player; also for future to indicate if player is in enemy range.


    public float bulletTimeIntervals = 1f; // Seconds to wait between each enemy bullet shot.
    private bool shootTimedown = false;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public int scoreToAddOnDeath = 10;

    private float time;
    private Transform player;
    private GameManager gm;

    public float detectionRadius = 10f;
    public LayerMask obstacleLayer;

    private PlayAudio pa;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        pa = GetComponent<PlayAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the direction towards the player
        Vector2 directionToPlayer = player.position - transform.position;

        // Perform a raycast to check for obstacles between the enemy and the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, obstacleLayer);

        // Check if the raycast hit the player
        if (hit.collider != null)
        {

            if (hit.collider.gameObject == player.gameObject || hit.collider.name == "Shield")
            {
                lookAtPlayer = true;
                //Debug.Log("Player is in line of sight!");
                // Do something when the player is in line of sight
            }
            else
            {
                lookAtPlayer = false;
            }
        }

        if (enemyMode == EnemyType.Melee)
        {
            /* // Only moving if line of sight to the player
            Vector2 dir = player.transform.position - transform.position;
            dir.Normalize();
            RaycastHit2D hitout = Physics2D.Raycast(transform.position, dir);
            if (hit)
            {
                //Debug.Log(hit.transform.name);
                if (hitout.transform.CompareTag("Player"))
                {
                   
                }
            } */           
            if (lookAtPlayer)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                float distance = speed * Time.deltaTime;

                transform.Translate(direction * distance);
            }
        }
        else if(enemyMode == EnemyType.Patrol) 
        {
            // Patrol over patrolPoints
            if (patrolPoints.Length == 0)
            {
                Debug.LogWarning("No patrol points assigned.");
                return;
            }

            Transform currentPatrolPoint = patrolPoints[currentPatrolIndex];
            Vector3 direction = (currentPatrolPoint.position - transform.position).normalized;
            float distance = speed * Time.deltaTime;

            transform.Translate(direction * distance);

            if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 0.1f)
            {
                if (isPatrollingForward)
                {
                    currentPatrolIndex++;
                    if (currentPatrolIndex >= patrolPoints.Length)
                    {
                        currentPatrolIndex = patrolPoints.Length - 2;
                        isPatrollingForward = false;
                    }
                }
                else
                {
                    currentPatrolIndex--;
                    if (currentPatrolIndex < 0)
                    {
                        currentPatrolIndex = 1;
                        isPatrollingForward = true;
                    }
                }
            }
        }

        if (lookAtPlayer)
        {
            // Look At Logic
            {
            // Get the direction from this object to the target
            Vector3 direction = player.position - transform.position;
            direction.Normalize();

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate the object around the Z-axis
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            // Shooting Logic
            time += Time.deltaTime;
            if (!shootTimedown) { BasicShoot(); }
            if (time > bulletTimeIntervals) 
            {
                time = 0;
                shootTimedown = false;
            }
        } else
        {
            // Look At Logic
            {
                // Get the direction from this object to the target
                Vector3 direction = player.position - transform.position;
                direction.Normalize();

                // Calculate the angle in degrees
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Rotate the object around the Z-axis
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the line of sight in the Scene view
        if (player != null)
        {
            Gizmos.color = lookAtPlayer ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    public void BasicShoot() 
    {       
        pa.PlayOneShotSound(0);
        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        shootTimedown = true; // Flag to indicate there should be timedown between each shot.
    }

    public void Die() 
    {
       
        gm.score += scoreToAddOnDeath;
        pa.PlayOneShotSound(1);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") 
        {
           // collision.gameObject.GetComponent<PlayerController>().
        }
    }


}
