using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletForce;
    public float destroyAfterSeconds = 2f;
    public float dmg = 5;

    private void Start()
    {
        // Apply force to the bullet in the forward direction
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
        
    }

    private void Update()
    {
        // Destroy the bullet after a certain time
        Destroy(gameObject, destroyAfterSeconds);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform != transform && !collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.transform.CompareTag("Player"))
            {
                Debug.Log(("DAMAGE", collision.transform.name));
                collision.transform.GetComponent<PlayerController>().Damage(dmg);
            }
            // Destroy the bullet upon collision with any object
            //Debug.Log(collision.transform.name);
            Destroy(gameObject);
        }
        
    }
}
