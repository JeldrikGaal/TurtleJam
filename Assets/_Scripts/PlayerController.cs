using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float Speed;
    [SerializeField] float acceleration = 1;
    [SerializeField] float Health = 100;
    [SerializeField] GameObject Shield;
    [SerializeField] SpriteRenderer ShieldSR;
    ShieldScript ShieldS;
    CameraManager cM;
    LineRenderer lR;

    public float radiusForShield = 1;
    Rigidbody2D rb;

    bool blockMovement;
    public bool shellReady;
    public bool shellFlying;

    public GameManager gM;

    bool aiming;

    Vector3 shieldDir;

    private float timeMod;

    // Boomerang variables
    [SerializeField] float flyingTime;
    [SerializeField] float flyingTimeBack;
    float flyingSpeed;
    Vector2 startingPos;
    Vector2 startingPosBack;
    Vector2 endPos;
    Vector2 endPosSave;
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
        if (Time.time - startingTime < (flyingTime * timeMod))
        {
            Shield.transform.position = Vector2.Lerp(startingPos, endPos, ( Time.time - startingTime ) / (flyingTime * timeMod));
        }
        else if (!flyingBack)
        {
            flyingBack = true;
            startingTimeBack = Time.time;
        }
        if (Time.time - startingTimeBack < (flyingTimeBack * timeMod) && flyingBack)
        {          
            Shield.transform.position = Vector2.Lerp(endPos, transform.position + (shieldDir * radiusForShield), (Time.time - startingTimeBack) / (flyingTimeBack * timeMod));
        }
        if (flyingBack && Time.time - startingTimeBack > (flyingTimeBack * timeMod))
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
        endPosSave = transform.position + (shieldDir * 10);
        ShieldS.ChangeState(1);

        Vector2 start = transform.position + (shieldDir);
        RaycastHit2D hit = Physics2D.Raycast(start, shieldDir);
        if (hit)
        {
            if (!hit.transform.CompareTag("Enemy"))
            {
                endPos = new Vector3(hit.point.x, hit.point.y) - (shieldDir * 0.25f);
            }
            else
            {
                endPos = new Vector3(hit.point.x, hit.point.y) + (shieldDir * 0.25f);
            }
        
            timeMod = Vector3.Distance(endPos, startingPos) / Vector3.Distance(endPosSave, startingPos);
        }
        else
        {
            timeMod = 1;
        }
       

    }

    private void Aiming()
    {
        float len = 10f;
        Vector2 start = transform.position + (shieldDir * 0.5f);
        Vector2 end = transform.position + (shieldDir * 11);

        float dist = Vector2.Distance(start, end);
        len = dist;
        float dist2;
        RaycastHit2D hit = Physics2D.Raycast(start, shieldDir);
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
                lR.SetPosition(i, start + direction * (len * ((i + 1f) / 10)));
        }
    }

    public void Damage(float dmg)
    {
        Health -= dmg;
        if (Health <= 0)
        {
            Debug.Log("You lost!");
            gM.paused = true;
        }
    }
}
