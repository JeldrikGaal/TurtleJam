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

    public void Activate(int currentStreak, bool streakBroken = false)
    {
        if (_streakThreshold > currentStreak)
        {
            Destroy(gameObject);
            return;
        }
        
        _seq.Append(transform.DOScale(Vector3.one * 1.5f, 0.25f));
        _seq.Append(transform.DOScale(Vector3.one, 0.125f));
        
        if (streakBroken)
        {
            gameObject.transform.SetParent(PlayerController.Instance.transform);
            _text.text = "Streak Over";
            _text.color = Color.red;
            StartCoroutine(StreakChangeColorFlash(_text.color));
            //_seq.Append(transform.DOScale(Vector3.one, 0.25f));
        }
        else
        {
            _text.color = GetColorFromStreak(currentStreak);
            if (currentStreak % 10 == 0)
            {
                SoundManager.PlayOneShotSound(SoundManager.Sound.GainStreak);
                StartCoroutine(StreakChangeColorFlash(_text.color));
            }
            _text.text = "Streak X" + currentStreak;
        }
        
        _seq.Append(transform.DOScale(Vector3.zero, 0.125f));
        _seq.OnComplete(() =>
        {
            Destroy(gameObject, 0.25f);
            _seq.Kill();
        });
    }

    private IEnumerator StreakChangeColorFlash(Color color)
    {
        ColorsController.Instance.StartGenericColorFlash(0.15f, color);
        yield return new WaitForSeconds(0.25f);
        ColorsController.Instance.StartGenericColorFlash(0.15f, color);
    }
}

