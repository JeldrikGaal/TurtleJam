using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StreakIndicator : MonoBehaviour
{
    [SerializeField] private int _streakThreshold;
    private TMP_Text _text;
    private Sequence _seq;

    [SerializeField] private List<ColorStreakAmountPair> _colorList;
    
    [Serializable]
    struct ColorStreakAmountPair
    {
        public Color Color;
        public int StreakAmount;
    }
    
    private void Awake()
    { 
        _text = GetComponent<TMP_Text>();
        _seq = DOTween.Sequence(); 
    }

    private Color GetColorFromStreak(int streakAmount)
    {
        for ( int i = _colorList.Count - 1; i > 0; i--)
        {
            if (streakAmount >= _colorList[i].StreakAmount)
            {
                return _colorList[i].Color;
            }
        }

        return _colorList[0].Color;
    }

    public void Activate(int currentStreak)
    {
        if (_streakThreshold > currentStreak)
        {
            Destroy(gameObject);
            return;
        }
        
        _text.text = "Streak X" + currentStreak;
        _text.color = GetColorFromStreak(currentStreak);

        
        _seq.Append(transform.DOScale(Vector3.one * 1.5f, 0.25f));
        _seq.Append(transform.DOScale(Vector3.one, 0.125f));
        _seq.Append(transform.DOScale(Vector3.zero, 0.125f));
        _seq.OnComplete(() =>
        {
            Destroy(gameObject, 0.25f);
            _seq.Kill();
        });
    }
}
