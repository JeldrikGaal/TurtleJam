using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System.Drawing;
using Color = UnityEngine.Color;
using System;

public class GameManager : MonoBehaviour
{
    public bool mainMenu = false;

    public int score = 0;
    public TextMeshProUGUI scoreTXT;

    public float time = 0;
    public TextMeshProUGUI timeTXT;

    public GameObject player;
    public bool paused = false;
    private List<Tilemap> tilemaps = new List<Tilemap>();
    //private Tilemap walls;
    private bool flashing;

    // To add
    // Sound logic
    // Level progression


    // Menus
    [Space(20)]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    // JUICE
    public GameObject mainCam;
    public GameObject epi;
    [SerializeField] public ParticleSystem explosion;

    // Cursor
    [Space(20)]
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    // wall color gradient
    [SerializeField] List<Color> colors = new List<Color>();
    [SerializeField] float gradientStepTime;
    int currentColor;

    float startTimeGradient;
    CameraManager cM;

    private bool levelComplete = false;
    private List<SpriteRenderer> introRoomText = new List<SpriteRenderer>();

    public TextMeshProUGUI finalScoreTXT;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        score = 0;
        time = 0;
        player = GameObject.FindWithTag("Player");
        mainCam = GameObject.FindWithTag("MainCamera");
        cM = mainCam.GetComponent<CameraManager>(); 
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Wall"))
        {
            tilemaps.Add(g.GetComponent<Tilemap>());
        }

        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);


        foreach (GameObject g in GameObject.FindGameObjectsWithTag("textToShift"))
        {
            introRoomText.Add(g.GetComponent<SpriteRenderer>());
        }



    }

    // Update is called once per frame
    void Update()
    {
        if (!levelComplete)
        {
            if (!mainMenu)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (!pauseMenu.activeInHierarchy )
                    {
                        Pause();
                    }
                    else
                    {
                        Resume();
                    }
                }
                time += Time.deltaTime;

                scoreTXT.text = "Score: " + score.ToString();
                timeTXT.text = "Time: " + time.ToString("0.00");
            }
            ColorShift();
        }
    }

    private void ColorShift()
    {
        if (!flashing)
        {
            float t = (Time.time - startTimeGradient) / gradientStepTime;
            if (t >= 1)
            {
                currentColor += 1;
                startTimeGradient = Time.time;
            }
            if (currentColor == colors.Count - 1 )
            {
                foreach (Tilemap ti in tilemaps)
                {
                    ti.color = Color.Lerp(colors[currentColor], colors[0], t);
                }
                if (introRoomText.Count > 0) 
                {
                    foreach(SpriteRenderer sR in introRoomText)
                    {
                        sR.color = Color.Lerp(colors[currentColor], colors[0], t);
                    }
                } 
                currentColor = 0;
            }
            else
            {
                foreach (Tilemap ti in tilemaps)
                {
                    ti.color = Color.Lerp(colors[currentColor], colors[currentColor + 1], t);
                }
                if (introRoomText.Count > 0)
                {
                    foreach (SpriteRenderer sR in introRoomText)
                    {
                        sR.color = Color.Lerp(colors[currentColor], colors[currentColor + 1], t);
                    }
                }
            }
            
            
        }
        
    }

    public void Pause() 
    {
        StartCoroutine(cM.BattleTransition(1, true));
        StartCoroutine(setTimeScaleDelayed(0, 1));
        StartCoroutine(setActiveDelayed(1, true));
        paused = true;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(cM.BattleTransition(1, false));
        
        paused = false;
    }

    public IEnumerator setActiveDelayed(float delay, bool active)
    {
        yield return new WaitForSeconds(delay);
        pauseMenu.SetActive(active);
    }
    public IEnumerator setActiveDelayedWin(float delay, bool active)
    {
        yield return new WaitForSeconds(delay);
        winMenu.SetActive(active);
    }


    public IEnumerator setTimeScaleDelayed(float timeScale, float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = timeScale;

    }

    public IEnumerator flashWalls(float time, Color color)
    {
        if (!flashing) 
        { 
            flashing = true;
            Color saveColor = Color.black;
            foreach (Tilemap ti in tilemaps)
            {
                saveColor = ti.color;
                ti.color = color;
            }

            yield return new WaitForSeconds(time);

            foreach (Tilemap ti in tilemaps)
            {
                ti.color = saveColor;
            }
            
            flashing = false;
        }
        
    }


    public void WinCondition() 
    {
        levelComplete = true;
        StartCoroutine(cM.BattleTransition(1, true));
        StartCoroutine(setActiveDelayedWin(1, true));

        int finalScore = (int)(100 - time) * 10 + score;
        finalScoreTXT.text = finalScore.ToString();
        // Save highscore somewhere.

        scoreTXT.enabled = false;
        timeTXT.enabled = false;
        paused = true;
    }

    public void GameOverCondition()
    {
        epi.GetComponent<Animator>().SetTrigger("GameOver");
        //mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
        //StartCoroutine(cM.BattleTransition(1, true));
        gameOverMenu.SetActive(true);
        player.GetComponent<PlayerController>();
        scoreTXT.enabled = false;
        timeTXT.enabled = false;
        paused = true;
    }

    public void GoToLevel(string levelName) // For Resume, Next Level and Back to Main Menu
    {
        if (levelName == "this")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        SceneManager.LoadScene(levelName);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }


}
