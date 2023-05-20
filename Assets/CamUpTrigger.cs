using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamUpTrigger : MonoBehaviour
{
    public GameObject camera;
    public float yOffset = 9.25f;
    private bool flagDone = false;

    Vector3 initialPos;
    float startingTime = 0;
    public float duration = 1f;
    bool doneMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        initialPos = camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!doneMoving && flagDone && camera.transform.position != new Vector3(initialPos.x, initialPos.y + yOffset, initialPos.z)) 
        {
            camera.transform.position = Vector3.Lerp(initialPos, new Vector3(initialPos.x, initialPos.y + yOffset, initialPos.z), Time.time - startingTime / duration);
        }else if(camera.transform.position == new Vector3(initialPos.x, initialPos.y + yOffset, initialPos.z)) { doneMoving = true;}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !flagDone) 
        {
            flagDone = true;
            startingTime = Time.time;
        }
    }
}
