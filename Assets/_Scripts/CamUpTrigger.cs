using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamUpTrigger : MonoBehaviour
{
    public enum direction
    {
        Up,
        Down,
        Right,
        Left
    }

    public direction movementDirection;

    public GameObject camera;
    public float yOffset = 9.25f;
    public float xOffset = 18.5f;

    private bool movingFlag = false;

    Vector3 initialPos;
    float startingTime = 0;
    public float duration = 1f;
    bool doneMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        initialPos = camera.transform.position;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (camera != null)
        {

            if (movementDirection == direction.Up)
            {
                if (!doneMoving && movingFlag && camera.transform.position != new Vector3(initialPos.x, initialPos.y + yOffset, initialPos.z))
                {
                    camera.transform.position = Vector3.Lerp(initialPos, new Vector3(initialPos.x, initialPos.y + yOffset, initialPos.z), Time.time - startingTime / duration);
                }
                else if (camera.transform.position == new Vector3(initialPos.x, initialPos.y + yOffset, initialPos.z)) { doneMoving = true; movingFlag = false; }
            }
            else if (movementDirection == direction.Down)
            {
                if (!doneMoving && movingFlag && camera.transform.position != new Vector3(initialPos.x, initialPos.y - yOffset, initialPos.z))
                {
                    camera.transform.position = Vector3.Lerp(initialPos, new Vector3(initialPos.x, initialPos.y - yOffset, initialPos.z), Time.time - startingTime / duration);
                }
                else if (camera.transform.position == new Vector3(initialPos.x, initialPos.y - yOffset, initialPos.z)) { doneMoving = true; movingFlag = false; }
            }
            else if (movementDirection == direction.Right)
            {
                if (!doneMoving && movingFlag && camera.transform.position != new Vector3(initialPos.x + xOffset, initialPos.y, initialPos.z))
                {
                    camera.transform.position = Vector3.Lerp(initialPos, new Vector3(initialPos.x + xOffset, initialPos.y, initialPos.z), Time.time - startingTime / duration);
                }
                else if (camera.transform.position == new Vector3(initialPos.x + xOffset, initialPos.y, initialPos.z)) { doneMoving = true; movingFlag = false; }
            }
            else if (movementDirection == direction.Left)
            {
                if (!doneMoving && movingFlag && camera.transform.position != new Vector3(initialPos.x - xOffset, initialPos.y, initialPos.z))
                {
                    camera.transform.position = Vector3.Lerp(initialPos, new Vector3(initialPos.x - xOffset, initialPos.y, initialPos.z), Time.time - startingTime / duration);
                }
                else if (camera.transform.position == new Vector3(initialPos.x - xOffset, initialPos.y, initialPos.z)) { doneMoving = true; movingFlag = false; }
            }
        } else
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !movingFlag) 
        {
            if (camera == null)
                camera = GameObject.FindGameObjectWithTag("MainCamera");
            initialPos = camera.transform.position;
            movingFlag = true;
            startingTime = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(FlipDirection());
        }
    }

    public IEnumerator FlipDirection() 
    {
        yield return new WaitForSeconds(1.1f);
        doneMoving = false;
        movingFlag = false;
        if (movementDirection == direction.Left) movementDirection = direction.Right;
        else if (movementDirection == direction.Right) movementDirection = direction.Left;
        else if (movementDirection == direction.Down) movementDirection = direction.Up;
        else if (movementDirection == direction.Up) movementDirection = direction.Down;
        Debug.Log("Direction flipped");

    }
}
