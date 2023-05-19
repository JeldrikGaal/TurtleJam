using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private bool shaking;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        //if (shaking) yield return null;

        float elapsedTime = 0f;
        Vector3 originalPos = transform.localPosition;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude;

            transform.localPosition = new Vector3(xOffset, yOffset, transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //shaking = false;
        transform.localPosition = originalPos;
    }
}
