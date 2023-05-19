using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletForce = 10f;
    public float destroyAfterSeconds = 2f;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet upon collision with any object
        Destroy(gameObject);
    }
}
