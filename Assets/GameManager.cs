using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public int score = 0;
    public TextMeshProUGUI scoreTXT;

    public float time = 0;
    public TextMeshProUGUI timeTXT;

    public GameObject player;

    // To add
    // Sound logic
    // Level progression
    // Menus


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        time = 0;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;

        scoreTXT.text = "Score: " + score.ToString();
        timeTXT.text = time.ToString("0.00");
    }
}
