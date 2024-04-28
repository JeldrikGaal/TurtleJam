using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;


public class SpawnerController : MonoBehaviour
{
    
    //public bool testBool;
    public int ActivationStage = 1;
    [SerializeField] private SpawnerInformationHolder _infoHolder;

    
    private GameObject currentSpawnedObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

        
    }

    public void InitializeSpawner()
    {
        if (currentSpawnedObject!= null) return;

        currentSpawnedObject = Instantiate(_infoHolder.ObjectToSpawn, this.transform.position, Quaternion.identity);

        currentSpawnedObject.SetActive(false);


    }
    public void ActivateSpawner(int DifficultyLevel)
    {
        if(currentSpawnedObject!= null){
            if (DifficultyLevel>= ActivationStage)
             currentSpawnedObject.SetActive(true);
             else currentSpawnedObject.SetActive(false);
        }        
    }

    public void ResetSpawner()
    {
        if(currentSpawnedObject != null)
        Destroy(currentSpawnedObject);
    }
        private void OnDrawGizmos()
    {
        if(_infoHolder != null)
        Gizmos.DrawIcon(transform.position, _infoHolder.sprite.name +".png", true);


    }

    #if UNITY_EDITOR

#endif
}
