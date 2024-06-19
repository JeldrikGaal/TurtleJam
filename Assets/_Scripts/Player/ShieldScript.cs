using UnityEngine;

public class ShieldScript : MonoBehaviour
{

    bool flying;
    public bool shielding;
    CameraManager cM;
    [SerializeField] ParticleSystem pS;
    [SerializeField] ParticleSystem pS2;
    TrailRenderer tR;
    Animator anim;
    BoxCollider2D bC;
    private PlayAudio _pa;

    // Start is called before the first frame update
    void Start()
    {
        cM = Camera.main.GetComponent<CameraManager>();
        tR = GetComponent<TrailRenderer>();
        anim = GetComponent<Animator>();
        bC = GetComponent<BoxCollider2D>();
        _pa = GetComponent<PlayAudio>();
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
                anim.SetTrigger("Idle");
                bC.enabled = false;
                break;
            case 1:
                flying = true;
                shielding = false;
                bC.enabled = true;
                break;
            case 2:
                flying = false;
                shielding = true;
                anim.SetTrigger("Shield");
                bC.enabled = true;
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
            //GetComponent<Animator>().SetBool("Shield", true);

            if (collision.CompareTag("Bullet") )
            {
                ParticleSystem ps = Instantiate(pS2, collision.transform.position, Quaternion.identity) as ParticleSystem;
                ps.transform.up = -1 * collision.transform.right;
                ps.Play();
                
                Destroy(ps.gameObject, 0.2f);
                //Debug.Log("destroy");
                Destroy(collision.gameObject);
                StartCoroutine(cM.Shake(0.05f, 0.2f));
            }
        }
        else GetComponent<Animator>().SetBool("Shield", false);

    }
}
