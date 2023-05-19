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
    
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyMode == EnemyType.Melee)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            float distance = speed * Time.deltaTime;

            transform.Translate(direction * distance);
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
        }
    }

    public void BasicShoot() 
    {
        GameObject newBullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        shootTimedown = true; // Flag to indicate there should be timedown between each shot.
    }

    public void Die() 
    {
        gm.score += scoreToAddOnDeath;
        Destroy(this.gameObject);
    }


}
