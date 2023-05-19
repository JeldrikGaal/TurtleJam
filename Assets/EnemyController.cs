using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public bool lookAtPlayer = true; // To indicate if the enemy should look at player; also for future to indicate if player is in enemy range.


    public float bulletTimeIntervals = 1f; // Seconds to wait between each enemy bullet shot.
    private bool shootTimedown = false;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    private float time;
    private Transform player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
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


}
