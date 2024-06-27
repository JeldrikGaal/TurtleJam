using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int _score;
    public float _timeSinceGameStarted;
   
    public ScoreManager _scoreManager;
    
    // Cursor
    [SerializeField] private Texture2D _cursorTexture;
    private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;
    
    [SerializeField] private GameObject _gameOverVisualEffectPrefab;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
        
        _scoreManager = GameObject.FindWithTag("UnityPlugin").GetComponent<ScoreManager>();
        
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.Running);
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
            if (GameStateManager.Instance.IsRunning())
            {
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Paused);
            }
            else
            {
                GameStateManager.Instance.SetGameState(GameStateManager.GameState.Running);
            }
        }

        if (GameStateManager.Instance.IsPaused())
        {
            return;
        }
        
        _timeSinceGameStarted += Time.deltaTime;
    }

    public void ResumeGame()
    {
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.Running);
    }

    public float CalculateScore()
    {
        return ((int)_timeSinceGameStarted * _score);
    }
    
    public void SaveScoreForPlayer(float score) 
    {
        _scoreManager.UpdateScore(score);
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
}
