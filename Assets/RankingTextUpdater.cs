using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingTextUpdater : MonoBehaviour
{
    public ScoreManager scoreManager;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (KeyValuePair<string, int> score in scoreManager.scores)
        {
            GetComponent<TextMeshProUGUI>().text = score.Key + " - " + score.Value.ToString() + " \n";
        }
         
    }


}
