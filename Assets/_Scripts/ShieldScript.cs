    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.UIElements;

public class ShieldScript : MonoBehaviour
{

    bool flying;
    bool shielding;
    CameraManager cM;
    [SerializeField] ParticleSystem pS;
    TrailRenderer tR;

    // Start is called before the first frame update
    void Start()
    {
        cM = Camera.main.GetComponent<CameraManager>();
        tR = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flying)
        {
            
            tR.enabled = true;
        }
        else
        {
            tR.Clear(); 
            tR.enabled = false;
            
        }
    }

    public void ChangeState(int state)
    {
        switch (state)
        {
            case 0:
                flying = false; 
                shielding = false;
                break;
            case 1:
                flying = true;
                shielding = false;
           
                break;
            case 2:
                flying = false;
                shielding = true;
                break;
        }
            

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (flying)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.GetComponent<EnemyController>().Die();
                //StartCoroutine(cM.Shake(0.05f, 0.2f));

                ParticleSystem ps = Instantiate(pS, collision.transform.position, Quaternion.identity) as ParticleSystem;
                ps.Play();
                Destroy(ps.gameObject, 0.2f);
            }
        }
        if (shielding)
        {
            Debug.Log(collision.name);

            if (!collision.CompareTag("Walls"))
            {
                StartCoroutine(cM.Shake(0.05f, 0.2f));
                Destroy(collision.gameObject);
            }

           
            
        }
        
    }
}