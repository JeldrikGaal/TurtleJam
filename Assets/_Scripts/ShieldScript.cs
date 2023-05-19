    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{

    bool flying;
    bool shielding;
    CameraManager cM;

    // Start is called before the first frame update
    void Start()
    {
        cM = Camera.main.GetComponent<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            }
        }
        if (shielding)
        {
            Debug.Log(collision.name);
            StartCoroutine(cM.Shake(0.05f, 0.2f));
            Destroy(collision.gameObject);
        }
        
    }
}
