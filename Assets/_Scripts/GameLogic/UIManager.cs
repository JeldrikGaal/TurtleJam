using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _powerUpTimeIndicatorBar;
    [SerializeField] private GameObject _powerUpTimeIndicatorContentHolder;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _powerUpHintText;
    [SerializeField] private TMP_Text _powerUpNameText;
    [SerializeField] private float _moveLengthPowerUpHintAnim;
    [SerializeField] private float _durationPowerUpHintAnim;
    
    [SerializeField] private Color _refreshTimerFlashColor;
    [SerializeField] private float _refreshTimerFlashDuration;
    [SerializeField] private float _refreshTimerScaleMultiplierDuration;
    
    [SerializeField] private TMP_Text _finalScoreText;
    [SerializeField] private TMP_Text _roomsCleardText;
    [SerializeField] private TMP_Text _streakBonusText;
    [SerializeField] private TMP_Text _enemiesKilledText;
    [SerializeField] private TMP_Text _timeDeductionText;
    

    // Menus
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _leaderboard;
    
    [SerializeField] private GameObject _gameOverVisualEffectPrefab;
    
    [SerializeField] private CameraManager _cameraManager;
    
    private Vector3 _scoreTextStartPos;
    private Vector3 _timeTextStartPos;
    private Vector3 _upgradeHintStartPos;

    private Sequence _upgradeHintSequenceStart;
    private Sequence _upgradeHintSequenceReturn;
    
    private Vector2 _powerUpTimeIndicatorStartPos;
    private Vector2 _powerUpTimeIndicatorHolderStartPos;
    private float _powerUpTimeIndicatorLength;
    private float _powerUpTimeIndicatorHolderLength;

    public GameManager gameManager;

    
    private static readonly int GameOver = Animator.StringToHash("GameOver");
    private void Awake()
    {
        GameStateManager.GameStateChanged += ReactToGameStateChange;
        PlayerController.OnPlayerDeath += GameOverCondition;
    }

    private void OnDestroy()
    {
        GameStateManager.GameStateChanged -= ReactToGameStateChange;
        PlayerController.OnPlayerDeath -= GameOverCondition;
    }

    private void Start ()
    {
        _powerUpTimeIndicatorStartPos = _powerUpTimeIndicatorBar.transform.localPosition;
        _powerUpTimeIndicatorHolderStartPos = _powerUpTimeIndicatorContentHolder.transform.localPosition;
        _powerUpTimeIndicatorLength = _powerUpTimeIndicatorBar.GetComponent<RectTransform>().rect.width;
        _powerUpTimeIndicatorHolderLength = _powerUpTimeIndicatorContentHolder.GetComponent<RectTransform>().rect.width;
        
        _scoreTextStartPos = _scoreText.transform.position;
        _timeTextStartPos = _timeText.transform.position;
        _upgradeHintStartPos = _powerUpHintText.transform.position;
        
    }

    private void Update()
    {
        if (GameStateManager.Instance.IsPaused())
        {
            return;
        }
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        _scoreText.text = "Score: " + gameManager._score;
        _timeText.text = "Time: " + gameManager._timeSinceGameStarted.ToString("0.00");
    }

    private void ReactToGameStateChange(GameStateManager.GameState newState)
    {
        switch (newState)
        {
            case GameStateManager.GameState.Paused:
                Pause();
                break;
            case GameStateManager.GameState.Running:
                Resume();
                break;
        }
    }

    private void Pause()
    {
        
        SoundManager.PauseAllPlayingSounds();
        SoundManager.PlayOneShotSound(SoundManager.Sound.StartGame);
        StartCoroutine(_cameraManager.BattleTransition(1, true));
        StartCoroutine(SetActiveDelayed(1, true));
    }
    
    public void Resume()
    {
        SoundManager.PlayOneShotSound(SoundManager.Sound.StartGame);
        SoundManager.ResumeAllPlayingSounds();
        _pauseMenu.SetActive(false);
        StartCoroutine(_cameraManager.BattleTransition(1, false));
    }
    
    public void GameOverCondition()
    {
        // TODO: find more fitting place !
        // TODO: rework after new leaderboard has been implemented
        SoundManager.StopAllPlayingSounds();
        SoundManager.PlayOneShotSound(SoundManager.Sound.PlayerDeath);
        _gameOverVisualEffectPrefab.GetComponent<Animator>().SetTrigger(GameOver);
        _gameOverMenu.SetActive(true);
        _scoreText.enabled = false;
        _timeText.enabled = false;
        
        gameManager.GetComponent<StatisticManager>().RegisterAnalytics();
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.GameOver);
        
        // Place the below code in a "CalculateFinalScore" function.
        
        gameManager.SaveScoreForPlayer(gameManager.CalculateScore());
        RunGameOverTextAnimation();
        //_finalScoreText.text = "Score \n" + gameManager.CalculateScore();


    }

    private void RunGameOverTextAnimation()
    {
        List<int> finalScoreBreakdown = GameManager.Instance.GetEndScoreBreakdownList();
        
        _finalScoreText.enabled = true;

        float duration = 0.5f;
        
        StartCoroutine(CountNumberUp(_enemiesKilledText,"Enemy Score: ", duration, 0, finalScoreBreakdown[0]));
        
        StartCoroutine(CountNumberUp(_streakBonusText,"Streak Score: ", duration, 0, finalScoreBreakdown[1], duration));
        
        StartCoroutine(CountNumberUp(_roomsCleardText,"Room Score: ", duration, 0, finalScoreBreakdown[2], duration * 2f));
        
        StartCoroutine(CountNumberUp(_timeDeductionText,"Time Score: ", duration, 0, finalScoreBreakdown[3], duration * 3f));
        
        StartCoroutine(CountNumberUp(_finalScoreText,"Total Score: ", duration, 0, gameManager.CalculateScore(), duration * 4f));
        
    }
    
    private IEnumerator CountNumberUp(TMP_Text textField,  string staticText, float duration, float start, float end, float waitTime = 0)
    {
        yield return new WaitForSeconds(waitTime + 0.25f);
        Vector3 saveScale = textField.transform.localScale;
        textField.gameObject.SetActive(true);
        textField.transform.localScale = Vector3.zero;
        textField.transform.DOScale(saveScale, 0.25f);
        textField.text = staticText;
        
        yield return new WaitForSeconds(0.25f);
        
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            textField.text = staticText + Mathf.Lerp(start, end, (elapsedTime / duration)).ToString("0");
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textField.text = staticText + end;

    }

    public void DisplayLeaderboard()
    {
        _leaderboard.SetActive(true);
    }
    
    public void HideLeaderboard()
    {
        _leaderboard.SetActive(false);
    }

    private IEnumerator SetActiveDelayed(float delay, bool active)
    {
        yield return new WaitForSeconds(delay);
        _pauseMenu.SetActive(active);
    }
    public void ShowPowerUpUI(BasePowerUpHolder data)
    {
        _powerUpNameText.text = data.DisplayName;
        _powerUpHintText.text = data.TutorialText;

        ShowPowerUpNameWithAnim();
        ShowUpgradeHintWithAnim(data.TutorialTextDuration);
    }

    private void ShowPowerUpNameWithAnim()
    {
        _powerUpTimeIndicatorContentHolder.transform.DOLocalMoveX(_powerUpTimeIndicatorHolderStartPos.x + _powerUpTimeIndicatorHolderLength, _durationPowerUpHintAnim).OnComplete(
            () =>
            {
                _powerUpTimeIndicatorBar.SetActive(true);
            });
    }

    public void HidePowerUpUI()
    { 
        _powerUpNameText.text = "";
        HidePowerUpNameWithAnim();
    }

    private void HidePowerUpNameWithAnim()
    {
        _powerUpTimeIndicatorBar.SetActive(false);
        _powerUpTimeIndicatorContentHolder.transform.DOLocalMoveX(_powerUpTimeIndicatorHolderStartPos.x, _durationPowerUpHintAnim);
    }

    private void ShowUpgradeHintWithAnim(float durationTillEnd)
    {
        _upgradeHintSequenceStart = DOTween.Sequence();
        _upgradeHintSequenceStart.Insert(0, _scoreText.transform.DOMoveX(_scoreTextStartPos.x + _moveLengthPowerUpHintAnim, _durationPowerUpHintAnim));
        _upgradeHintSequenceStart.Insert(0, _powerUpHintText.transform.DOMoveX(_upgradeHintStartPos.x + _moveLengthPowerUpHintAnim, _durationPowerUpHintAnim));
        _upgradeHintSequenceStart.Insert(0, _timeText.transform.DOMoveX(_timeTextStartPos.x + _moveLengthPowerUpHintAnim, _durationPowerUpHintAnim));
        
        Invoke(nameof(HideUpgradeHintWithAnim), durationTillEnd);

    }
    
    private void HideUpgradeHintWithAnim()
    {
        _upgradeHintSequenceReturn = DOTween.Sequence();
        _upgradeHintSequenceReturn.Insert(0, _scoreText.transform.DOMoveX(_scoreTextStartPos.x, _durationPowerUpHintAnim));
        _upgradeHintSequenceReturn.Insert(0, _powerUpHintText.transform.DOMoveX(_upgradeHintStartPos.x , _durationPowerUpHintAnim));
        _upgradeHintSequenceReturn.Insert(0, _timeText.transform.DOMoveX(_timeTextStartPos.x, _durationPowerUpHintAnim));
    }
    
    public void PositionBarToPercent(float percentage)
    {
        _powerUpTimeIndicatorBar.transform.localPosition = new Vector3(_powerUpTimeIndicatorStartPos.x + _powerUpTimeIndicatorLength * percentage, _powerUpTimeIndicatorStartPos.y);
    }

    public void RefreshTimerEffect()
    {
        Image loadingBarImage = _powerUpTimeIndicatorBar.GetComponent<Image>();
        Color saveColor = loadingBarImage.color;
        loadingBarImage.color = _refreshTimerFlashColor;

        loadingBarImage.transform.DOPunchScale(loadingBarImage.transform.localScale * _refreshTimerScaleMultiplierDuration, _refreshTimerFlashDuration).OnComplete(() =>
        {
            loadingBarImage.color = saveColor;
        });
    }
    public void PlayButtonSound(){
        SoundManager.PlayOneShotSound(SoundManager.Sound.ButtonSelect);
    }
}
