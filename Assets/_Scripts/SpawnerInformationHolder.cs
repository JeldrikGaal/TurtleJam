using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/SpawnerInfoHolder")]
public class SpawnerInformationHolder : ScriptableObject
{
    public enum SpawnerType
    {
        EnemySpawner,
        InteractableSpawner,
    }

    public GameObject ObjectToSpawn;
    public SpawnerType Type;
    

   
}
