using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColorsController : MonoBehaviour
{
    // Objects
    private List<Tilemap> _tileMaps = new List<Tilemap>();
    private List<SpriteRenderer> _worldTextRenderers = new List<SpriteRenderer>();
    
    // Flashing
    private bool _currentlyFlashingColors;
    [SerializeField] private float _projectileFlashDuration;
    [SerializeField] private Color _projectileFlashColor;
    
    // Shifting
    [SerializeField] private List<Color> _colorShiftColors = new List<Color>();
    [SerializeField] private float _colorShiftDuration;
    private float _colorShiftDurationMod = 1;
    
    private int _currentColorIndex;
    private float _startTimeColorShift;
    
    // Streak speed increase

    [SerializeField]
    private List<StreakSpeedPair> _streakSettings;
    
    [Serializable]
    private struct StreakSpeedPair
    {
        public int RequiredStreakCount;
        [Range(0,1)]
        public float SpeedIncrease;
    }
    
    public static ColorsController Instance;

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

        LevelController.TileMapsChanged += UpdateObjectsToShift;
        StreakLogic.StreakReached += UpdateSpeedModFromStreak;
    }

    private void OnDestroy()
    {
        LevelController.TileMapsChanged -= UpdateObjectsToShift;
        StreakLogic.StreakReached -= UpdateSpeedModFromStreak;
    }

    private void Start()
    {
        _startTimeColorShift = Time.time;
        Invoke(nameof(FetchObjectsToColorShift), 0.05f);
    }
    
    private void FetchObjectsToColorShift()
    {
        Debug.Log("FETCHED");
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Wall"))
        {
            _tileMaps.Add(g.GetComponent<Tilemap>());
        }
        
        // TODO: rethink if this will be fine in the future
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("textToShift"))
        {
            _worldTextRenderers.Add(g.GetComponent<SpriteRenderer>());
        }
    }

    private void Update()
    {
        if (GameStateManager.Instance.IsPaused())
        {
            return;
        }
        ColorShift();
    }

    private void ColorShift()
    {
        if (_currentlyFlashingColors)
        {
            return;
        }
        
        float t = (Time.time - _startTimeColorShift) / ( _colorShiftDuration * _colorShiftDurationMod ) ;
        
        int targetColorIndex = _currentColorIndex ==  _colorShiftColors.Count - 1? 0 : _currentColorIndex + 1;
        
        ChangeTileMapsColor( Color.Lerp(_colorShiftColors[_currentColorIndex], _colorShiftColors[targetColorIndex], t));
        ChangeInWorldTextColor(Color.Lerp(_colorShiftColors[_currentColorIndex], _colorShiftColors[targetColorIndex], t));

        if (t >= 1)
        {
            _currentColorIndex = targetColorIndex;
            _startTimeColorShift = Time.time;
        }

    }
    
    private void ChangeTileMapsColor(Color newColor)
    {
        foreach (Tilemap ti in _tileMaps)
        {
            ti.color = newColor;
        }
    }
    
    private void ChangeInWorldTextColor(Color newColor)
    {
        if (_worldTextRenderers.Count <= 0)
        {
            return;
        }
        
        foreach(SpriteRenderer sR in _worldTextRenderers)
        {
            sR.color = newColor;
        }
    }
    
    private void UpdateObjectsToShift(List<GameObject> stageHolders)
    {
        _tileMaps = new List<Tilemap>();
        _worldTextRenderers = new List<SpriteRenderer>();

        foreach (var g in stageHolders)
        {
            foreach (Transform child in g.transform)
            {
                if (child.CompareTag("Wall"))
                {
                    _tileMaps.Add(child.GetComponent<Tilemap>());
                }
                if (child.CompareTag("textToShift"))
                {
                    _worldTextRenderers.AddRange(child.GetComponentsInChildren<SpriteRenderer>());
                }
            }
        }
        
    }

    public void StartProjectileColorFlash()
    {
        StartCoroutine(FlashWalls(_projectileFlashDuration, _projectileFlashColor));
    }

    public void RegisterRangeToColorShift(List<SpriteRenderer> renderers)
    {
        _worldTextRenderers.AddRange(renderers);
    }
    
    private IEnumerator FlashWalls(float flashDuration, Color flashColor)
    {
        // Only allow to start flashing if currently not flashing
        if (_currentlyFlashingColors) yield break;
        
        _currentlyFlashingColors = true;
        Color saveColor = _tileMaps[0].color;
        ChangeTileMapsColor(flashColor);
        ChangeInWorldTextColor(flashColor);

        yield return new WaitForSeconds(flashDuration);

        ChangeTileMapsColor(saveColor);
        ChangeInWorldTextColor(saveColor);
        _currentlyFlashingColors = false;
    }

    public void InformAboutObjectDestruction(GameObject destroyedObject)
    {
        if (_tileMaps.Contains(destroyedObject.GetComponent<Tilemap>()))
        {
            _tileMaps.Remove(destroyedObject.GetComponent<Tilemap>());
        }
        else if (_worldTextRenderers.Contains(destroyedObject.GetComponent<SpriteRenderer>()) )
        {
            _worldTextRenderers.Remove(destroyedObject.GetComponent<SpriteRenderer>());
        }
    }

    private void UpdateSpeedModFromStreak(int streakAmount)
    {
        _colorShiftDurationMod = GetSpeedModFromStreak(streakAmount);
    }
    
    private float GetSpeedModFromStreak(int streakAmount)
    {
        if (streakAmount == 0)
        {
            return 1;
        }
        for ( int i = _streakSettings.Count - 1; i > 0; i--)
        {
            if (streakAmount >= _streakSettings[i].RequiredStreakCount)
            {
                return _streakSettings[i].SpeedIncrease;
            }
        }

        return _streakSettings[0].SpeedIncrease;
    }
}
