using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private bool _cameraShakeRunning;
    [SerializeField] private BattleTransitionEffect _battleTransitionEffect;

    private bool _transitioning = false; // for in-rooms transitions.
    private float _startingTime; // used for lerp.
    private float _duration; // duration of level transitions
    private Vector3 _posA;
    private Vector3 _posB;

    public static CameraManager Instance;

    private Vector3 _currentCamGoal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _currentCamGoal = transform.position;
    }
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        /*if (_transitioning && Vector3.Distance(_posA,_posB) >= 0.1f)
        {
            gameObject.transform.position = Vector3.Lerp(_posA, _posB, Time.time - _startingTime / _duration);       
        }
        else { _transitioning = false; }*/
    } 
    
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsedTime = 0f;
        Vector3 originalPos = transform.localPosition;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x+xOffset, originalPos.y + yOffset, transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //shaking = false;
        transform.position = _currentCamGoal;
    }

    public void FreezeFrames(float time)
    {
        Time.timeScale = 0;
        StartCoroutine(nameof(ResetTimeScale), time);
    }

    IEnumerator ResetTimeScale(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
       
    }
    
    public IEnumerator BattleTransition(float time, bool open)
    {
        float start = 1f;
        float end = 0;
        if (open)
        {
            start = 0;
            end = 1f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            _battleTransitionEffect.Cutoff = Mathf.Lerp(start, end, (elapsedTime  / time));
            elapsedTime += Time.deltaTime;
            
            //Debug.Log(Time.deltaTime);
            yield return null;
        }
    }

    public void CameraTransition(Vector3 posB, float time = 1f)
    {
        /*_posA = this.gameObject.transform.position;
        _posB = posB;
        _duration = time;
        _startingTime = Time.time;
        _transitioning = true; // Triggers lerp in Update () */
        _currentCamGoal = posB;
        transform.DOMove(posB, time);
    }
}