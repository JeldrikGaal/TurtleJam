using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BulletController : MonoBehaviour
{
    public float bulletForce;
    public float destroyAfterSeconds = 2f;
    public float dmg = 5;

    [SerializeField] ParticleSystem muzzleFlash;

    private void Start()
    {
        // Apply force to the bullet in the forward direction
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * bulletForce, ForceMode2D.Impulse);
        ParticleSystem ps = Instantiate(muzzleFlash, transform.position, Quaternion.identity) as ParticleSystem;
        ps.transform.up = transform.right;
        ps.Play();
        
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
                collision.transform.GetComponent<PlayerController>().Damage(dmg);
            }
            if (!collision.transform.CompareTag("Bullet"))
            {
                Destroy(gameObject);
            }
            // Destroy the bullet upon collision with any object
            //Debug.Log(collision.transform.name);


        }
        
    }
}
