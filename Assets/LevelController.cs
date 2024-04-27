using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject currentRoom; // This variable represents the latest room that has been generated. and NOT the actual current room player is in. MUST BE Set to initial room first in inspector. 
    
    public int numOfRooms = 3;
    [Tooltip("Levels that their main entrance comes from the bottom")]
    public List<GameObject> levelsDown;
    [Tooltip("Levels that their main entrance comes from the top")]
    public List<GameObject> levelsUp;
    [Tooltip("Levels that their main entrance comes from the right")]
    public List<GameObject> levelsRight;
    [Tooltip("Levels that their main entrance comes from the left")]
    public List<GameObject> levelsLeft;

    public float yOffset;
    public float xOffset;

    private Transform grid;
    public List<Transform> generatedRooms;

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").transform;
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        for(int i = 0; i < numOfRooms; i++) 
        {
            if (currentRoom.GetComponent<LevelAttributes>().hasExitUp && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have down && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsDown.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsDown[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsDown[j]);
                    else if (i != numOfRooms - 1 && !levelsDown[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsDown[j]);
                    
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x, currentRoom.transform.localPosition.y + yOffset, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;
            }
            else if (currentRoom.GetComponent<LevelAttributes>().hasExitRight && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have left && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsLeft.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsLeft[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsLeft[j]);
                    else if (i != numOfRooms - 1 && !levelsLeft[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsLeft[j]);
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x + xOffset, currentRoom.transform.localPosition.y, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }
            else if (currentRoom.GetComponent<LevelAttributes>().hasExitLeft && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have right && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsRight.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsRight[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsRight[j]);
                    else if (i != numOfRooms - 1 && !levelsRight[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsRight[j]);
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x - xOffset, currentRoom.transform.localPosition.y, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }

            else if (currentRoom.GetComponent<LevelAttributes>().hasExitDown && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have up && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levelsUp.Count; j++)
                {
                    if (i == numOfRooms - 1 && levelsUp[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsUp[j]);
                    else if (i != numOfRooms - 1 && !levelsUp[j].GetComponent<LevelAttributes>().isEndScreen)
                        availableRooms.Add(levelsUp[j]);
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x, currentRoom.transform.localPosition.y - yOffset, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }
        }
    }
}