using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public enum type
    {
        Slowdown,
        Speedup
    }

    public type interactableType;

    public float duration = 3f;
    public bool activated = false;

    private float time = 0;

    private void Update()
    {
        if (activated) 
        {
            time += Time.deltaTime;
            if (time >= duration) 
            {
                if (interactableType == type.Slowdown)
                    DeactivateSlowDown();
                else if (interactableType == type.Speedup)
                    DeactivateSpeedUp();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        activated = true;
        if (interactableType == type.Slowdown)
            ActivateSlowdown();
        else ActivateSpeedUp();
    }

    void ActivateSlowdown() 
    {
        Time.timeScale = 0.5f;
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
    }

    void DeactivateSlowDown() 
    {
        Time.timeScale = 1f;
        Destroy(this.gameObject);
    }

    void ActivateSpeedUp()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;

        GameObject.FindWithTag("Player").GetComponent<PlayerController>().Speed *= 1.5f;
    }

    void DeactivateSpeedUp()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().Speed /= 1.5f;
        Destroy(this.gameObject);
    }


}
