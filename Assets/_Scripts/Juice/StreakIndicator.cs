using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StreakIndicator : MonoBehaviour
{
    [SerializeField] private int _streakThreshold;
    private TMP_Text _text;
    private Sequence _seq;
    
    private void Awake()
    { 
        _text = GetComponent<TMP_Text>();
        _seq = DOTween.Sequence(); 
    }

    public void Activate(string text, Color color)
    {
        _text.text = text;
        _text.color = color;
        
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

