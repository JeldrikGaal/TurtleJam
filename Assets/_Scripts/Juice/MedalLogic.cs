using DG.Tweening;
using TMPro;
using UnityEngine;

public class MedalLogic : MonoBehaviour
{
    [SerializeField] private TMP_Text _letterText;
    [SerializeField] private SpriteRenderer _medalRenderer;

    [SerializeField] private float _flyInAnimationTime;
    [SerializeField] private float _overshootAnimationTime;
    [SerializeField] private float _flyInDistance;
    [SerializeField] private float _overshootDistance;

    [SerializeField] private float _shakeImpactDuration;
    [SerializeField] private float _shakeImpactMagnitude;

    public void SetupMedal(MedalProvider.MedalInfo medalInfo)
    {
        _letterText.text = medalInfo.Letter;
        _medalRenderer.color = medalInfo.Color;
    }

    public void ActivateMedal(Vector3 endPosition)
    {
        Sequence seq = DOTween.Sequence();
        transform.position = endPosition + ( Vector3.up *_flyInDistance) ;
        seq.Append(transform.DOMoveY(endPosition.y - _overshootDistance, _flyInAnimationTime));
        seq.Append(transform.DOMoveY(endPosition.y, _overshootAnimationTime));
        seq.OnComplete(() =>
        {
            StartCoroutine(CameraManager.Instance.Shake(_shakeImpactDuration, _shakeImpactMagnitude));
            ColorsController.Instance.StartGenericColorFlash(0.05f, _medalRenderer.color);
        });
    }

}
