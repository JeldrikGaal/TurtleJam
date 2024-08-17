using DG.Tweening;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    private bool _settingsOpen;
    private bool _creditsOpen;
    
    [SerializeField] private GameObject _settingsHolder;
    [SerializeField] private GameObject _menuHolder;
    [SerializeField] private GameObject _titleHolder;
    [SerializeField] private GameObject _creditsHolder;
    
    
    [SerializeField]  private float _settingsAnimXOffset;
    [SerializeField]  private float _settingsAnimDuration;
    
    [SerializeField]  private float _creditsAnimYOffset;
    [SerializeField]  private float _creditssAnimDuration;

    private Vector3 _settingsStartPos;
    private Vector3 _menuStartPos;
    private Vector3 _titleStartPos;
    private Vector3 _creditsStartPos;


    private void Awake()
    {
        _settingsStartPos = _settingsHolder.transform.localPosition;
        _menuStartPos = _menuHolder.transform.localPosition;
        _titleStartPos = _titleHolder.transform.localPosition;
        _creditsStartPos = _creditsHolder.transform.localPosition;

    }

    public void OpenSettings()
    {
        if (_settingsOpen)
        {
            return;
        }

        _settingsOpen = true;
        
        _menuHolder.transform.DOLocalMoveX(_menuStartPos.x - _settingsAnimXOffset, _settingsAnimDuration);
        _titleHolder.transform.DOLocalMoveX(_titleStartPos.x - _settingsAnimXOffset, _settingsAnimDuration);
        
        _settingsHolder.transform.localPosition += Vector3.right * _settingsAnimXOffset;
        _settingsHolder.SetActive(true);
        _settingsHolder.transform.DOLocalMoveX(_settingsStartPos.x, _settingsAnimDuration);
    }

    public void CloseSettings()
    {
        if (!_settingsOpen)
        {
            return;
        }

        _settingsOpen = false;
        
        _menuHolder.transform.DOLocalMoveX(_menuStartPos.x, _settingsAnimDuration);
        _titleHolder.transform.DOLocalMoveX(_titleStartPos.x, _settingsAnimDuration);
        
        _settingsHolder.transform.DOLocalMoveX(_settingsStartPos.x + _settingsAnimXOffset, _settingsAnimDuration).OnComplete(() =>
        {
            _settingsHolder.SetActive(false);
        });
    }

    public void ShowCredits()
    {
        if (_creditsOpen)
        {
            return;
        }

        _creditsOpen = true;
        
        _settingsHolder.transform.DOLocalMoveY(_settingsStartPos.y + _creditsAnimYOffset, _creditssAnimDuration);
        
        _creditsHolder.transform.localPosition -= Vector3.up * _settingsAnimXOffset;
        _creditsHolder.SetActive(true);
        _creditsHolder.transform.DOLocalMoveY(_creditsStartPos.y, _creditssAnimDuration);
    }
    
    public void HideCredits()
    {
        if (!_creditsOpen)
        {
            return;
        }

        _creditsOpen = false;
        
        _settingsHolder.transform.DOLocalMoveY(_settingsStartPos.y, _creditssAnimDuration);
        
        _creditsHolder.transform.DOLocalMoveY(_creditsStartPos.y - _settingsAnimXOffset, _creditssAnimDuration).OnComplete(
            () =>
            {
                _creditsHolder.SetActive(false);
            });
    }
}
