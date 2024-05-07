using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

public class GameManager : MonoBehaviour
{

    private int _score = 0;
    private float _timeSinceGameStarted = 0;
    
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private TMP_Text _timeText;
    
    private bool _paused = false;
    private List<Tilemap> _tileMaps = new List<Tilemap>();
    private bool _currentlyFlashingColors;
    private ScoreManager _scoreManager;
    
    // Menus
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;

    // JUICE
    [SerializeField] private GameObject _gameOverVisualEffectPrefab;

    // Cursor
    [SerializeField] private Texture2D _cursorTexture;
    private const CursorMode CursorMode = UnityEngine.CursorMode.Auto;
    private Vector2 _hotSpot = Vector2.zero;

    // wall color shifting
    [SerializeField] private List<Color> _colorShiftColors = new List<Color>();
    [SerializeField] private float _colorShiftDuration;
    private int _currentColorIndex;
    private float _startTimeColorShift;
    
    private CameraManager _cameraManager;
    private List<SpriteRenderer> _worldTextRenderers = new List<SpriteRenderer>();
    
    void Start()
    {
        InitializeValuesAndReferences();
        FetchObjectsToColorShift();
        SetupCursor();
    }

    private void InitializeValuesAndReferences()
    {
        Time.timeScale = 1;
        _score = 0;
        _timeSinceGameStarted = 0;

        _startTimeColorShift = Time.time;
        
        _cameraManager = GameObject.FindWithTag("MainCamera").GetComponent<CameraManager>();
        _scoreManager = gameObject.GetComponent<ScoreManager>();
    }
    
    private void FetchObjectsToColorShift()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Wall"))
        {
            _tileMaps.Add(g.GetComponent<Tilemap>());
        }
        
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("textToShift"))
        {
            _worldTextRenderers.Add(g.GetComponent<SpriteRenderer>());
        }
    }
    
    private void SetupCursor()
    {
        _hotSpot = new Vector2(_cursorTexture.width / 2f, _cursorTexture.height / 2f);
        Cursor.SetCursor(_cursorTexture, _hotSpot, CursorMode);
    }
    
    void Update()
    {
        ColorShift();
        
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

    public void UpdateTileMapList()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Tilemap t = g.GetComponent<Tilemap>();
            if (!_tileMaps.Contains(t))
            {
                _tileMaps.Add(t);
            }
        }
    }

    private void ColorShift()
    {
        if (_currentlyFlashingColors) return;
        
        float t = (Time.time - _startTimeColorShift) / _colorShiftDuration;
        
        int targetColorIndex = _currentColorIndex ==  _colorShiftColors.Count - 1? 0 : _currentColorIndex + 1;
        
        ChangeTileMapsColor( Color.Lerp(_colorShiftColors[_currentColorIndex], _colorShiftColors[targetColorIndex], t));
        ChangeInWorldTextColor(Color.Lerp(_colorShiftColors[_currentColorIndex], _colorShiftColors[targetColorIndex], t));

        if (t >= 1)
        {
            _currentColorIndex = targetColorIndex;
            _startTimeColorShift = Time.time;
        }

    }

    private void ChangeInWorldTextColor(Color newColor)
    {
        if (_worldTextRenderers.Count > 0) 
        {
            foreach(SpriteRenderer sR in _worldTextRenderers)
            {
                sR.color = newColor;
            }
        }
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

    public IEnumerator FlashWalls(float flashDuration, Color flashColor)
    {
        // Only allow to start flashing if currently not flashing
        if (_currentlyFlashingColors) yield break;
        
        _currentlyFlashingColors = true;
        Color saveColor = _tileMaps[0].color;
        ChangeTileMapsColor(flashColor);

        yield return new WaitForSeconds(flashDuration);

        ChangeTileMapsColor(saveColor);
        _currentlyFlashingColors = false;
    }

    private void ChangeTileMapsColor(Color newColor)
    {
        foreach (Tilemap ti in _tileMaps)
        {
            ti.color = newColor;
        }
    }

    public void SaveScoreForPlayer() 
    {
        // TODO: uncommented for testing 
        //scoreManager.UpdateScore(score); 
        
        //SaveNewScore(highscoreName.text, (int)((int)(100 - time) * 10 + score));
        //highscoreSection.SetActive(false);
    }

    public void GameOverCondition()
    {
        // TODO: rework after new leaderboard has been implemented
        _gameOverVisualEffectPrefab.GetComponent<Animator>().SetTrigger("GameOver");
        _gameOverMenu.SetActive(true);
        _scoreText.enabled = false;
        _timeText.enabled = false;
        _paused = true;

        _score = ((int)_timeSinceGameStarted * _score); // Score calculation
        SaveScoreForPlayer();
        _finalScoreText.text = "Score \n" + _score.ToString();
        _finalScoreText.enabled = true;
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
