using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class FadeFromBlack : MonoBehaviour
{
    [SerializeField] private RawImage _image;

    private void Awake()
    {
        _image.enabled = true;
        _image.DOFade(0, 1f).OnComplete(() =>
        {
            Destroy(gameObject, 0.1f);
        });
    }
}
