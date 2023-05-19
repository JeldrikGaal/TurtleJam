using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float Speed;
    [SerializeField] float acceleration = 1;
    [SerializeField] GameObject Shield;
    [SerializeField] SpriteRenderer ShieldSR;

    float radiusForShield = 1;
    Rigidbody2D rb;

    bool blockMovement;
    public bool shellReady;
    public bool shellFlying;

    Vector3 shieldDir;

    // Boomerang variables
    [SerializeField] float flyingTime;
    [SerializeField] float flyingTimeBack;
    float flyingSpeed;
    Vector2 startingPos;
    Vector2 startingPosBack;
    Vector2 endPos;
    float startingTime;
    float startingTimeBack;
    bool flyingBack;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ShieldSR = Shield.GetComponent<SpriteRenderer>();
        shellReady = true;
    }

    // Update is called once per frame
    void Update()
    {

        // Shield positioning 
        shieldDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        shieldDir = new Vector3(shieldDir.x, shieldDir.y, 0);
        shieldDir.Normalize();
        Shield.transform.position = transform.position + (shieldDir * radiusForShield);

        // Movement input
        rb.velocity = Vector3.zero;
        
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(Speed, rb.velocity.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-Speed, rb.velocity.y);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, Speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector2(rb.velocity.x, -Speed);
        }

        

        // Handling Right Click
        if (Input.GetMouseButtonDown(1))
        {
            if (shellReady)
            {
                blockMovement = true;
                ShieldSR.color = Color.red;
            }
          
        }       
        if (Input.GetMouseButtonUp(1))
        {
            if (shellReady)
            {
                blockMovement = false;
                ShieldSR.color = Color.green;
            }
        }

        // Handling Left Click
        if (Input.GetMouseButtonDown(0))
        {
            if (shellReady)
            {
                BoomerangShot();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            
        }

        if (shellFlying)
        {
            BommerangLogic();
        }


        // Needs to be at the end of update
        if (blockMovement)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void BommerangLogic()
    {
        if (Time.time - startingTime < flyingTime)
        {
            Shield.transform.position = Vector2.Lerp(startingPos, endPos, ( Time.time - startingTime ) / flyingTime);
            Debug.Log("1");
        }
        else if (!flyingBack)
        {
            flyingBack = true;
            startingTimeBack = Time.time;
        }
        if (Time.time - startingTimeBack < flyingTimeBack && flyingBack)
        {          
            Shield.transform.position = Vector2.Lerp(endPos, startingPos, (Time.time - startingTimeBack) / flyingTimeBack);
        }
        if (flyingBack && Time.time - startingTimeBack > flyingTimeBack)
        {
            flyingBack = false;
            shellFlying = false;
        }
        
    }

    private void BoomerangShot()
    {
        shellFlying = true;
        startingPos = Shield.transform.position;
        startingTime = Time.time;
        endPos = shieldDir * 10;
    }
}
