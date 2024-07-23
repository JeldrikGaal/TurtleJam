using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BarDisplay : MonoBehaviour
{
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    
    [SerializeField] private List<RawImage> _images;
    [SerializeField] private GameObject _barPrefab;

    private int _currentBarAmount;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StepUp();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StepDown();
        }
    }

    public void Initialize(int amount)
    {
        SetListFromAmount(amount);
    }

    private int GetAmountFromPercentage(float percentage)
    {
        return Mathf.RoundToInt(_images.Count * percentage);
    }
    
    public void SetListFromAmount(int amount)
    {
        _currentBarAmount = amount;
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].color = i < amount ? _activeColor : _inactiveColor;
        }
    }

    public void SetListFromAmountWithAnim(int amount)
    {
        if (amount > _currentBarAmount && amount <= _images.Count)
        {
            EnableBar(_images[amount-1]);
        }
        else if (amount < _currentBarAmount && amount >= 0)
        {
            DisableBar(_images[amount]);
        }
        
        _currentBarAmount = amount;
        //StartCoroutine(SetListFromAmountWithDelay(amount));
    }

    private IEnumerator SetListFromAmountWithDelay(int amount)
    {
        yield return new WaitForSeconds(0.35f);
        SetListFromAmount(amount);
    }
    
    private void EnableBar(RawImage image)
    {
        Transform imageTransform = image.transform;
        var fakeBar = GameObject.Instantiate(_barPrefab, imageTransform);
        fakeBar.transform.localPosition = Vector3.zero;
        fakeBar.transform.localScale = Vector3.zero;
        fakeBar.GetComponent<RawImage>().color = _activeColor;
        fakeBar.transform.DOScale(Vector3.one * 1.125f, 0.25f).OnComplete(() =>
        {
            fakeBar.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                image.color = _activeColor;
                Destroy(fakeBar);
            });
        });
        
    }

    private void DisableBar(RawImage image)
    {
        Transform imageTransform = image.transform;
        var fakeBar = GameObject.Instantiate(_barPrefab, imageTransform);
        fakeBar.transform.localPosition = Vector3.zero;
        fakeBar.GetComponent<RawImage>().color = _activeColor;
        image.color = _inactiveColor;
        
        fakeBar.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
        {
            Destroy(fakeBar);
        });
    }

    public void StepUp()
    {
        if (_currentBarAmount >= _images.Count)
        {
            return;
        }
        EnableBar(_images[_currentBarAmount]);
        _currentBarAmount++;
    }

    public void StepDown()
    {
        if (_currentBarAmount <= 0)
        {
            return;
        }
        _currentBarAmount--;
        DisableBar(_images[_currentBarAmount]);
    }
}
