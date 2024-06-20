using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int _score = 0;
    private float _timeSinceGameStarted = 0;
    
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private TMP_Text _timeText;
    
    private bool _paused = false;
   
    private ScoreManager _scoreManager;
    
    // Menus
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _leaderboard;

    // JUICE
    [SerializeField] private GameObject _gameOverVisualEffectPrefab;

    // Cursor
    [SerializeField] private Texture2D _cursorTexture;
    private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;
    
    private CameraManager _cameraManager;
    
    private void Start()
    {
        InitializeValuesAndReferences();
        SetupCursor();
    }

    private void InitializeValuesAndReferences()
    {
        Time.timeScale = 1;
        _score = 0;
        _timeSinceGameStarted = 0;
        
        _cameraManager = GameObject.FindWithTag("MainCamera").GetComponent<CameraManager>();
        _scoreManager = GameObject.FindWithTag("UnityPlugin").GetComponent<ScoreManager>();
    }
    
    private void SetupCursor()
    {
        _hotSpot = new Vector2(_cursorTexture.width / 2f, _cursorTexture.height / 2f);
        Cursor.SetCursor(_cursorTexture, _hotSpot, CursorMode);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_pauseMenu.activeInHierarchy )
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
        _timeSinceGameStarted += Time.deltaTime;

        _scoreText.text = "Score: " + _score.ToString();
        _timeText.text = "Time: " + _timeSinceGameStarted.ToString("0.00");
        
    }

    private void Pause()
    {
        StartCoroutine(_cameraManager.BattleTransition(1, true));
        StartCoroutine(SetTimeScaleDelayed(0, 1));
        StartCoroutine(SetActiveDelayed(1, true));
        _paused = true;
    }

    private IEnumerator SetActiveDelayed(float delay, bool active)
    {
        yield return new WaitForSeconds(delay);
        _pauseMenu.SetActive(active);
    }

    private IEnumerator SetTimeScaleDelayed(float timeScale, float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = timeScale;

    }
    
    public void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(_cameraManager.BattleTransition(1, false));
        
        _paused = false;
    }

    public void SaveScoreForPlayer() 
    {
        _scoreManager.UpdateScore(_score);
    }

    public void GameOverCondition()
    {
        // TODO: rework after new leaderboard has been implemented
        _gameOverVisualEffectPrefab.GetComponent<Animator>().SetTrigger("GameOver");
        _gameOverMenu.SetActive(true);
        _scoreText.enabled = false;
        _timeText.enabled = false;
        _paused = true;

        
        // Place the below code in a "CalculateFinalScore" function.
        _score = ((int)_timeSinceGameStarted * _score); // Score calculation
        SaveScoreForPlayer();
        _finalScoreText.text = "Score \n" + _score.ToString();
        _finalScoreText.enabled = true;
    }

    public void DisplayLeaderboard()
    {
        _leaderboard.SetActive(true);
        _scoreManager.UpdateScoresOnLeaderboard();
    }
    
    public void HideLeaderboard()
    {
        _leaderboard.SetActive(false);
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


    public void AddScore(int scoreToAdd)
    {
        _score += scoreToAdd;
    }

    public bool IsPaused()
    {
        return _paused;
    }
}
