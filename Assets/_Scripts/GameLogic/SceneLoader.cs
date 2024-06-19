using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    Transform cam;

    [SerializeField] Vector3 posGoal;
    [SerializeField] Vector3 scaleGoal;
    [SerializeField] string level1Name = "Jeldrik2";
    [SerializeField] float introSequenceTime;


    Vector3 posStart;
    Vector3 scaleStart;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("MainMenu").transform;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string levelName) // For Resume, Next Level and Back to Main Menu
    {
        if (levelName == "this") SceneManager.LoadScene(SceneManager.loadedSceneCount);
        SceneManager.LoadScene(levelName);
    }

    public void StartIntro()
    {
        StartCoroutine(Intro());
    }

    public IEnumerator Intro ()
    {
        float elapsedTime = 0;
        posStart = cam.transform.localPosition;
        scaleStart = cam.transform.localScale;

        while (elapsedTime < introSequenceTime)
        {
            cam.transform.localPosition = Vector3.Lerp(posStart, posGoal, (elapsedTime / introSequenceTime));
            cam.localScale = Vector3.Lerp(scaleStart, scaleGoal, (elapsedTime / introSequenceTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        LoadScene(level1Name);
    }

    public void Close()
    {
        Application.Quit();
    }
}
