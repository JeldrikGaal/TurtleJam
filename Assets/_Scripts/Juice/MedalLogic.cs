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
    [SerializeField] private Color _medalFlashColor;

    private GameObject _correspondingTextObject;

    public void SetupMedal(MedalProvider.MedalInfo medalInfo, GameObject textObject)
    {
        _letterText.text = medalInfo.Letter;
        _medalRenderer.color = medalInfo.Color;
        _correspondingTextObject = textObject;
    }

    public void ActivateMedal(Vector3 endPosition)
    {

       _correspondingTextObject.SetActive(true);
        
        Sequence seq = DOTween.Sequence();
        transform.position = endPosition + ( Vector3.up *_flyInDistance) ;
        seq.Append(transform.DOMoveY(endPosition.y - _overshootDistance, _flyInAnimationTime));
        seq.Append(transform.DOMoveY(endPosition.y, _overshootAnimationTime));
        seq.OnComplete(() =>
        {
            StartCoroutine(CameraManager.Instance.Shake(_shakeImpactDuration, _shakeImpactMagnitude));
            ColorsController.Instance.StartGenericColorFlash(0.05f, _medalFlashColor);
        });
        
        Sequence seq2 = DOTween.Sequence();

        Vector3 textStartPos = _correspondingTextObject.transform.position;
        _correspondingTextObject.transform.position = textStartPos + ( Vector3.up *_flyInDistance) ;
        seq2.Append(_correspondingTextObject.transform.DOMoveY(textStartPos.y - _overshootDistance, _flyInAnimationTime));
        seq2.Append(_correspondingTextObject.transform.DOMoveY(textStartPos.y, _overshootAnimationTime));
    }

}
