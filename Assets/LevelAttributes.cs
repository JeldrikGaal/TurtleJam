using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAttributes : MonoBehaviour
{

    [System.Flags]
    public enum hasExits
    {
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        // Add more options as needed
    }
    public hasExits selectedExits;

    private void Start()
    {
        
    }

    private void Update()
    {
        // Check if a specific option is selected
        if ((selectedExits & hasExits.Up) != 0)
        {
            Debug.Log("It has an exit up top!");
        }
    }
}
