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

    
    private GameObject _currentSpawnedObject;
    private Vector3 _debugLableOffset = Vector3.down;

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
        if (_currentSpawnedObject!= null) return;
        
        _currentSpawnedObject = Instantiate(_infoHolder.ObjectToSpawn, this.transform.position, Quaternion.identity);

        _currentSpawnedObject.SetActive(false);


    }
    public void ActivateSpawner(int difficultyLevel)
    {
        if(_currentSpawnedObject != null)
        {
            if (difficultyLevel >= ActivationStage)
            {
                _currentSpawnedObject.SetActive(true);
            }
            else
            {
                _currentSpawnedObject.SetActive(false);
            }
        }        
    }


    public void ResetSpawner()
    {
        if(_currentSpawnedObject != null)
        Destroy(_currentSpawnedObject);
    }
        private void OnDrawGizmos()
    {
        if(_infoHolder != null){
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position +_debugLableOffset, ActivationStage.ToString());
#endif
            Gizmos.DrawIcon(transform.position , _infoHolder.sprite.name +".png", true);
            
        }

    }

   
}
