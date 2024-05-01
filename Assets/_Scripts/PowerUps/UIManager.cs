using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingBar;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _upgradeHintText;
    [SerializeField] private TMP_Text _upgradeNameText;
    
    private Vector2 _startPos;
    private float _length;
    
    private void Start ()
    {
        _startPos = _loadingBar.transform.localPosition;
        _length = _loadingBar.GetComponent<RectTransform>().rect.width;
    }

    public void ShowUI(BasePowerUpHolder data)
    {
        _loadingBar.SetActive(true);
        _upgradeNameText.gameObject.SetActive(true);
        _upgradeNameText.text = data.DisplayName;
    }

    public void HideUI()
    { 
        _loadingBar.SetActive(false);
        _upgradeNameText.gameObject.SetActive(false);
        _upgradeNameText.text = "";
    }

    private void ShowUpgradeHintWithAnim()
    {
        
    }
    
    private void HideUpgradeHintWithAnim()
    {
        
    }
    
    public void PositionBarToPercent(float percentage)
    {
        _loadingBar.transform.localPosition = new Vector3(_startPos.x + _length * percentage, _startPos.y);
    }
}
