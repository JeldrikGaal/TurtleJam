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
    ShieldScript ShieldS;
    CameraManager cM;
    LineRenderer lR;

    float radiusForShield = 1;
    Rigidbody2D rb;

    bool blockMovement;
    public bool shellReady;
    public bool shellFlying;

    public GameManager gM;

    bool aiming;

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
        ShieldS = Shield.GetComponent<ShieldScript>();
        shellReady = true;
        cM = Camera.main.GetComponent<CameraManager>();
        lR = GetComponent<LineRenderer>();
        gM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gM.paused) return;
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
                ShieldS.ChangeState(2);
                shellReady = false;
            }
          
        }       
        if (Input.GetMouseButtonUp(1))
        {
            if (!shellReady)
            {
                blockMovement = false;
                ShieldSR.color = Color.green;
                ShieldS.ChangeState(0);
                shellReady = true;
            }

        }

        // Handling Left Click
        if (Input.GetMouseButtonDown(0))
        {
            aiming = true;
            lR.enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (shellReady)
            {
                BoomerangShot();
               
            }
            aiming = false;
            lR.enabled = false;
        }
        if (shellFlying)
        {
            BommerangLogic();
        }

        if (aiming)
        {
            Aiming();
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
        }
        else if (!flyingBack)
        {
            flyingBack = true;
            startingTimeBack = Time.time;
        }
        if (Time.time - startingTimeBack < flyingTimeBack && flyingBack)
        {          
            Shield.transform.position = Vector2.Lerp(endPos, transform.position + (shieldDir * radiusForShield), (Time.time - startingTimeBack) / flyingTimeBack);
        }
        if (flyingBack && Time.time - startingTimeBack > flyingTimeBack)
        {
            flyingBack = false;
            shellFlying = false;
            shellReady = true;
            ShieldS.ChangeState(0);
        }
        
    }

    private void BoomerangShot()
    {
        shellFlying = true;
        shellReady = false;
        startingPos = Shield.transform.position;
        startingTime = Time.time;
        endPos = transform.position + (shieldDir * 10);
        ShieldS.ChangeState(1);
    }

    private void Aiming()
    {
        Vector2 start = transform.position;
        Vector2 end = transform.position + (shieldDir * 10);

        float dist = Vector2.Distance(start, end);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, shieldDir);
        if (hit && !hit.transform.CompareTag("Player") || !hit.transform.CompareTag("Shield"))
        {
            if (Vector2.Distance(transform.position,hit.transform.position) < dist)
            {
                end = hit.transform.position;
            }
        }

        Vector2 direction = end - start;
        direction.Normalize();

        for (int i = 0; i < 10; i++)
        {
                lR.SetPosition(i, start + direction * (dist * ((i + 1f) / 10f)));
        }
    }
}
