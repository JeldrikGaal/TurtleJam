using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public enum type
    {
        Slowdown,
        Speedup,
        PixilizeScreen
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
            Debug.Log(time);
            if (time >= duration) 
            {
                if (interactableType == type.Slowdown)
                    DeactivateSlowDown();
                else if (interactableType == type.Speedup)
                    DeactivateSpeedUp();
                else if (interactableType == type.PixilizeScreen)
                    DeactivatePixelation();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        activated = true;
        if (interactableType == type.Slowdown) ActivateSlowdown();
        else if(interactableType == type.Speedup) ActivateSpeedUp();
        else if(interactableType == type.PixilizeScreen) ActivatePixelation();
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

    void ActivatePixelation() 
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
    }

    void DeactivatePixelation() 
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().mainCam.GetComponent<PixelationEffect>().AnimatePixelationIn();
        Destroy(this.gameObject);
    }


}
