using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpUI : MonoBehaviour
{
    [SerializeField] private GameObject _loadingBar;
    [SerializeField] private TMP_Text _upgradeText;
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
        _upgradeText.gameObject.SetActive(true);
        _upgradeText.text = data.DisplayName;
    }

    public void HideUI()
    { 
        _loadingBar.SetActive(false);
        _upgradeText.gameObject.SetActive(false);
        _upgradeText.text = "";
    }
    
    public void PositionBarToPercent(float percentage)
    {
        _loadingBar.transform.localPosition = new Vector3(_startPos.x + _length * percentage, _startPos.y);
    }


}
