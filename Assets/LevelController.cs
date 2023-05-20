using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameObject currentRoom; // Set it to initial room first.

    public int numOfRooms = 3;
    public List<GameObject> levels;

    public float yOffset;
    public float xOffset;

    private Transform grid;
    private List<Transform> generatedRooms;

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").transform;
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        for(int i = 0; i < numOfRooms; i++) 
        {
            if (currentRoom.GetComponent<LevelAttributes>().up && !currentRoom.GetComponent<LevelAttributes>().nextRoomConnected)
            {
                // Look for all rooms which have down && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levels.Count; j++)
                {
                    if (levels[j].GetComponent<LevelAttributes>().down)
                    {
                        if (i == numOfRooms - 1 && levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                        else if (i != numOfRooms - 1 && !levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                    }
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x, currentRoom.transform.localPosition.y + yOffset, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;
            }
            else if (currentRoom.GetComponent<LevelAttributes>().right)
            {
                // Look for all rooms which have left && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levels.Count; j++)
                {
                    if (levels[j].GetComponent<LevelAttributes>().left)
                    {
                        if (i == numOfRooms - 1 && levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                        else if (i != numOfRooms - 1 && !levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                    }
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x + xOffset, currentRoom.transform.localPosition.y, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }
            else if (currentRoom.GetComponent<LevelAttributes>().left)
            {
                // Look for all rooms which have right && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levels.Count; j++)
                {
                    if (levels[j].GetComponent<LevelAttributes>().right)
                    {
                        if (i == numOfRooms - 1 && levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                        else if (i != numOfRooms - 1 && !levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                    }
                }


                // Randomly selecting a room and instantiating it
                GameObject newRoomSelected = availableRooms[Random.RandomRange(0, availableRooms.Count - 1)];
                GameObject instantiatedRoom = Instantiate(newRoomSelected, grid);
                instantiatedRoom.transform.localPosition = new Vector3(currentRoom.transform.localPosition.x - xOffset, currentRoom.transform.localPosition.y, currentRoom.transform.localPosition.z);

                currentRoom.GetComponent<LevelAttributes>().nextRoomConnected = instantiatedRoom;
                currentRoom = instantiatedRoom;

            }

            else if (currentRoom.GetComponent<LevelAttributes>().down)
            {
                // Look for all rooms which have up && not the end scene.
                List<GameObject> availableRooms = new List<GameObject>();
                for (int j = 0; j < levels.Count; j++)
                {
                    if (levels[j].GetComponent<LevelAttributes>().up)
                    {
                        if (i == numOfRooms - 1 && levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                        else if (i != numOfRooms - 1 && !levels[j].GetComponent<LevelAttributes>().endScreen)
                            availableRooms.Add(levels[j]);
                    }
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