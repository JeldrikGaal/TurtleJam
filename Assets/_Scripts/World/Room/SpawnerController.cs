using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpawnerController : MonoBehaviour
{
    
    //public bool testBool;
    public int ActivationStage = 1;
    [Header("Use single element for determined spawn and list for random spawning")]
    [SerializeField] private SpawnerInformationHolder _infoHolder;
    [SerializeField] private List<SpawnerInformationHolder> _infoHolderList;

    
    private GameObject _currentSpawnedObject;
    private Vector3 _debugLableOffset = Vector3.down;


    public void InitializeSpawner()
    {
        if (_currentSpawnedObject != null) return;
        
        if (_infoHolderList.Count == 0)
        {
            _infoHolderList.Add(_infoHolder);
        }
        SpawnerInformationHolder infoHolder = GetRandomInfoHolder(); 
        
        Debug.Log(infoHolder.ObjectToSpawn);
        _currentSpawnedObject = Instantiate(infoHolder.ObjectToSpawn, this.transform.position, Quaternion.identity);

        _currentSpawnedObject.SetActive(false);

       
        
    }

    private SpawnerInformationHolder GetRandomInfoHolder()
    {
        return _infoHolderList[Random.Range(0, _infoHolderList.Count)];
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
        if (_currentSpawnedObject != null)
        {
            Destroy(_currentSpawnedObject);
        }
        
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_infoHolder!=null)
        {
            Gizmos.DrawIcon(transform.position , _infoHolder.sprite.name +".png", true);
            
                UnityEditor.Handles.Label(transform.position +_debugLableOffset, ActivationStage.ToString());
        }
    }
    #endif

}
