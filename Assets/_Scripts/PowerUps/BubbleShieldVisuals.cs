using DG.Tweening;
using UnityEngine;

public class BubbleShieldVisuals : MonoBehaviour
{
    private CameraManager _cameraManager;

    private void Awake()
    {
        PlayerController.NewBubbleShieldValueJustDropped += ShieldEndedVisuals;
    }

    private void OnDisable()
    {
        PlayerController.NewBubbleShieldValueJustDropped -= ShieldEndedVisuals;
    }

    private void Start()
    {
        transform.DOPunchScale(transform.localScale * 0.25f, 5, 1, 1).SetLoops(-1);
        _cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    private void ShieldEndedVisuals(int newLevel)
    {
        if (newLevel == 0)
        {
            StartCoroutine(_cameraManager.Shake(0.2f, 0.5f));
        }
        
    }
    
    
}
