using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingBar;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _upgradeHintText;
    [SerializeField] private TMP_Text _upgradeNameText;
    [SerializeField] private float _moveLengthUpgradHintAnim;
    [SerializeField] private float _durationUpgradeHintAnim;
    
    [SerializeField] private Color _refreshTimerFlashColor;
    [SerializeField] private float _refreshTimerFlashDuration;
    [SerializeField] private float _refreshTimerScaleMultiplierDuration;

    private Vector3 _scoreTextStartPos;
    private Vector3 _timeTextStartPos;
    private Vector3 _upgradeHintStartPos;

    private Sequence _upgradeHintSequenceStart;
    private Sequence _upgradeHintSequenceReturn;
    
    private Vector2 _startPos;
    private float _length;
    
    private void Start ()
    {
        _startPos = _loadingBar.transform.localPosition;
        _length = _loadingBar.GetComponent<RectTransform>().rect.width;
        
        _scoreTextStartPos = _scoreText.transform.position;
        _timeTextStartPos = _timeText.transform.position;
        _upgradeHintStartPos = _upgradeHintText.transform.position;

        _upgradeHintSequenceStart = DOTween.Sequence();
        _upgradeHintSequenceReturn = DOTween.Sequence();
    }

    public void ShowPowerUpUI(BasePowerUpHolder data)
    {
        _loadingBar.SetActive(true);
        _upgradeNameText.gameObject.SetActive(true);
        _upgradeNameText.text = data.DisplayName;

        _upgradeHintText.text = data.TutorialText;

        ShowUpgradeHintWithAnim(data.TutorialTextDuration);
    }

    public void HidePowerUpUI()
    { 
        _loadingBar.SetActive(false);
        _upgradeNameText.gameObject.SetActive(false);
        _upgradeNameText.text = "";
    }

    private void ShowUpgradeHintWithAnim(float durationTillEnd)
    {
        _upgradeHintSequenceStart.Insert(0, _scoreText.transform.DOMoveX(_scoreTextStartPos.x + _moveLengthUpgradHintAnim, _durationUpgradeHintAnim));
        _upgradeHintSequenceStart.Insert(0, _upgradeHintText.transform.DOMoveX(_upgradeHintStartPos.x + _moveLengthUpgradHintAnim, _durationUpgradeHintAnim));
        _upgradeHintSequenceStart.Insert(0, _timeText.transform.DOMoveX(_timeTextStartPos.x + _moveLengthUpgradHintAnim, _durationUpgradeHintAnim));
        
        Invoke(nameof(HideUpgradeHintWithAnim), durationTillEnd);

    }
    
    private void HideUpgradeHintWithAnim()
    {
        _upgradeHintSequenceReturn.Insert(0, _scoreText.transform.DOMoveX(_scoreTextStartPos.x, _durationUpgradeHintAnim));
        _upgradeHintSequenceReturn.Insert(0, _upgradeHintText.transform.DOMoveX(_upgradeHintStartPos.x , _durationUpgradeHintAnim));
        _upgradeHintSequenceReturn.Insert(0, _timeText.transform.DOMoveX(_timeTextStartPos.x, _durationUpgradeHintAnim));
    }
    
    public void PositionBarToPercent(float percentage)
    {
        _loadingBar.transform.localPosition = new Vector3(_startPos.x + _length * percentage, _startPos.y);
    }

    public void RefreshTimerEffect()
    {
        Image loadingBarImage = _loadingBar.GetComponent<Image>();
        Color saveColor = loadingBarImage.color;
        loadingBarImage.color = _refreshTimerFlashColor;

        loadingBarImage.transform.DOPunchScale(loadingBarImage.transform.localScale * _refreshTimerScaleMultiplierDuration, _refreshTimerFlashDuration).OnComplete(() =>
        {
            loadingBarImage.color = saveColor;
        });
    }
}
