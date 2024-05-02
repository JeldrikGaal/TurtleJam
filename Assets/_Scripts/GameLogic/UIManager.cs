using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    private Vector3 _scoreTextStartPos;
    private Vector3 _timeTextStartPos;
    private Vector3 _upgradeHintStartPos;

    private Sequence _upgradeHintSequenceStart;
    private Sequence _upgradeHintSequenceReturn;
    
    private Vector2 _powerUpTimeIndicatorStartPos;
    private Vector2 _powerUpTimeIndicatorHolderStartPos;
    private float _powerUpTimeIndicatorLength;
    private float _powerUpTimeIndicatorHolderLength;
    
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
}
