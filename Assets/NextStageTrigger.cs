using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    private LevelController _lc;
    private bool _stageUpdated = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _lc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !_stageUpdated)
        {
            _stageUpdated = true;
            //_lc.ProgressToNextStage();
        }
    }
}
