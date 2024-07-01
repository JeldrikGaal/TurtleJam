using DG.Tweening;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360);
            //transform.DOScale(Vector3.zero, 1f);
            other.transform.DOMove(transform.position, 1f);
            other.transform.DOScale(Vector3.zero, 1f).OnComplete(() =>
            { 
                other.transform.GetComponent<PlayerController>().Kill();
            });           
        }
    }
}
