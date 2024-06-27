using System.Collections;
using System.Collections.Generic;
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
    private int _currentColorIndex;
    private float _startTimeColorShift;
    
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
    }

    private void OnDestroy()
    {
        LevelController.TileMapsChanged -= UpdateObjectsToShift;
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
        Debug.Log("CLEARED");
        _tileMaps = new List<Tilemap>();
        _worldTextRenderers = new List<SpriteRenderer>();
        foreach (GameObject g in stageHolders)
        {
            _tileMaps.AddRange(g.GetComponentsInChildren<Tilemap>());
            _worldTextRenderers.AddRange(g.GetComponentsInChildren<SpriteRenderer>());
        }
    }

    public void StartProjectileColorFlash()
    {
        StartCoroutine(FlashWalls(_projectileFlashDuration, _projectileFlashColor));
    }
    
    private IEnumerator FlashWalls(float flashDuration, Color flashColor)
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
}
