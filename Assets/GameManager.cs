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
    private GameObject mainCam;
    public GameObject epi;
    [SerializeField] public ParticleSystem explosion;

    // wall color gradient
    [SerializeField] List<Color> colors = new List<Color>();
    float gradientStepTime;
    int currentColor;

    float startTimeGradient;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        time = 0;
        player = GameObject.FindWithTag("Player");
        mainCam = GameObject.FindWithTag("MainCamera");
        walls = GameObject.FindWithTag("Wall").GetComponent<Tilemap>();
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

    private void ColorShift()
    {
        if (!flashing)
        {
            float t = 0;
            if (currentColor == colors.Count - 1 )
            {
                //walls.color = Color.Lerp(colors[currentColor], colors[0]);
            }
            else
            {
                //walls.color = Color.Lerp(colors[currentColor], colors[0]);
            }
            
        }
        
    }

    public void Pause() 
    {
        mainCam.GetComponent<PixelationEffect>().AnimatePixelationOut();
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        paused = true;
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
