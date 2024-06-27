using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpawnerController : MonoBehaviour
{
    
    //public bool testBool;
    public int ActivationStage = 1;

    public int IgnoreStage = -1; 
    
    [Header("Use single element for determined spawn and list for random spawning")]
    [SerializeField] private SpawnerInformationHolder _infoHolder;
    [SerializeField] private List<SpawnerInformationHolder> _infoHolderList;
    
    private GameObject _currentSpawnedObject;
    private Vector3 _debugLableOffset = Vector3.down;

    private LevelAttributes _levelAttributes;

    private void Awake()
    {
        _levelAttributes = transform.parent.GetComponent<LevelAttributes>();
        if (!_levelAttributes)
        {
            Debug.LogError("Can't have spawner outside of room");
        }
    }

    public void InitializeSpawner()
    {
        if (_currentSpawnedObject != null) return;
        
        if (_infoHolderList.Count == 0)
        {
            _infoHolderList.Add(_infoHolder);
        }
        SpawnerInformationHolder infoHolder = GetRandomInfoHolder(); 
        
        _currentSpawnedObject = Instantiate(infoHolder.ObjectToSpawn, transform.position, Quaternion.identity, _levelAttributes.transform);

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
            if (difficultyLevel >= ActivationStage && difficultyLevel != IgnoreStage)
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
            
            Handles.Label(transform.position +_debugLableOffset, ActivationStage.ToString());
        }
    }
    #endif

}
