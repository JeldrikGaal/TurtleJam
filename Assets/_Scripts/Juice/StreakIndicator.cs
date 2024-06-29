using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StreakIndicator : MonoBehaviour
{
    private TMP_Text _text;
    private void Awake()
    {

        _text = GetComponent<TMP_Text>();
    }

    public void Activate(int currentStreak)
    {
        _text.text = "Streak X" + currentStreak;
        transform.DOScale(Vector3.one * 1.5f, 0.25f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.125f).OnComplete(() =>
            {
                Destroy(gameObject, 0.25f);
            });
        });
    }
}
