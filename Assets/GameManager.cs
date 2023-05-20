using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool mainMenu = false;

    public int score = 0;
    public TextMeshProUGUI scoreTXT;

    public float time = 0;
    public TextMeshProUGUI timeTXT;

    public GameObject player;
    public bool paused = false;

    // To add
    // Sound logic
    // Level progression


    // Menus
    [Space(20)]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    // JUICE
    private GameObject mainCam;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        time = 0;
        player = GameObject.FindWithTag("Player");
        mainCam = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Pause();

            time += Time.deltaTime;

            scoreTXT.text = "Score: " + score.ToString();
            timeTXT.text = time.ToString("0.00");
        }
    }

    public void Pause() 
    {
        mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        paused = true;
    }

    public void Resume()
    {
        mainCam.GetComponent<PixelationEffect>().AnimatePixelationIn();

        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        paused = false;
    }

    public void WinCondition() 
    {
        mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
        winMenu.SetActive(true);
    }

    public void GameOverCondition()
    {
        mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
        gameOverMenu.SetActive(true);
    }

    public void GoToLevel(string levelName) // For Resume, Next Level and Back to Main Menu
    {
        if(levelName == "this") SceneManager.LoadScene(SceneManager.loadedSceneCount);
        SceneManager.LoadScene(levelName);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }


}
