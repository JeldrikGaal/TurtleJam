using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private bool shaking;
    private BattleTransitionEffect bE;

    // Start is called before the first frame update
    void Start()
    {
        bE = transform.GetComponent<BattleTransitionEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(BattleTransition(1, true));
        }  
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(BattleTransition(1, false));
        }
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

            transform.localPosition = new Vector3(originalPos.x+xOffset, originalPos.y + yOffset, transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //shaking = false;
        transform.localPosition = originalPos;
    }

    public IEnumerator BattleTransition(float time, bool open)
    {

        float start = 0.5f;
        float end = 0;
        if (open)
        {
            start = 0;
            end = 0.5f;
        }
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            bE.Cutoff = Mathf.Lerp(start, end, (elapsedTime  / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
