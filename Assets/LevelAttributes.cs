using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAttributes : MonoBehaviour
{

/*    [System.Flags]
    public enum hasExits
    {
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        EndScreen = 1 << 4
        // Add more options as needed
    }

    [SerializeField]
    public hasExits selectedExits;*/

    public bool hasExitUp;
    public bool hasExitDown;
    public bool hasExitLeft;
    public bool hasExitRight;
    public bool isEndScreen;

    public GameObject nextRoomConnected;

}
