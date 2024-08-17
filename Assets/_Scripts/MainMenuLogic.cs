using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    private bool _settingsOpen;
    [SerializeField] private GameObject _settingsHolder;
    [SerializeField] private GameObject _menuHolder;
    [SerializeField] private GameObject _title;
    [SerializeField]  private float _settingsAnimXOffset;
    [SerializeField]  private float _settingsAnimDuration;

    private Vector3 _settingsStartPos;
    private Vector3 _menuStartPos;
    private Vector3 _titleStartPos;


    private void Awake()
    {
        _settingsStartPos = _settingsHolder.transform.localPosition;
        _menuStartPos = _menuHolder.transform.localPosition;
        _titleStartPos = _title.transform.localPosition;

    }

    public void OpenSettings()
    {
        if (_settingsOpen)
        {
            return;
        }

        _settingsOpen = true;
        
        _menuHolder.transform.DOLocalMoveX(_menuStartPos.x - _settingsAnimXOffset, _settingsAnimDuration);
        _title.transform.DOLocalMoveX(_titleStartPos.x - _settingsAnimXOffset, _settingsAnimDuration);
        
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
        _title.transform.DOLocalMoveX(_titleStartPos.x, _settingsAnimDuration);
        _settingsHolder.transform.DOLocalMoveX(_settingsStartPos.x + _settingsAnimXOffset, _settingsAnimDuration).OnComplete(() =>
        {
            _settingsHolder.SetActive(false);
        });
    }
}
