using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public bool mainMenu = false;

    public int score = 0;
    public TextMeshProUGUI scoreTXT;

    public float time = 0;
    public TextMeshProUGUI timeTXT;

    public GameObject player;
    public bool paused = false;
    private Tilemap walls;
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

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        time = 0;
        player = GameObject.FindWithTag("Player");
        mainCam = GameObject.FindWithTag("MainCamera");
        cM = mainCam.GetComponent<CameraManager>(); 
        walls = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();

        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Pause();

            time += Time.deltaTime;

            scoreTXT.text = "Score: " + score.ToString();
            timeTXT.text = "Time: " + time.ToString("0.00");
        }
        ColorShift();
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
                walls.color = Color.Lerp(colors[currentColor], colors[0], t);
                currentColor = 0;
            }
            else
            {
                walls.color = Color.Lerp(colors[currentColor], colors[currentColor + 1], t);
            }
            
            
        }
        
    }

    public void Pause() 
    {
        //mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
        StartCoroutine(cM.BattleTransition(1, true));
        pauseMenu.SetActive(true);
        StartCoroutine(setTimeScaleDelayed(0, 1));
        //Time.timeScale = 0;
        paused = true;
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
            Color saveColor = walls.color;
            walls.color = color;
            yield return new WaitForSeconds(time);
            walls.color = saveColor;
            flashing = false;
        }
        
    }

    public void Resume()
    {
        //mainCam.GetComponent<PixelationEffect>().AnimatePixelationIn();
        StartCoroutine(cM.BattleTransition(1, false));
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
        epi.GetComponent<Animator>().SetTrigger("GameOver");
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
