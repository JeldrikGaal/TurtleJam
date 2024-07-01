using System.Collections;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject loginScreen;
    
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameObject usernameInput;
    
    Transform cam;

    [SerializeField] Vector3 posGoal;
    [SerializeField] Vector3 scaleGoal;
    [SerializeField] string level1Name = "Level";
    [SerializeField] float introSequenceTime;


    Vector3 posStart;
    Vector3 scaleStart;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        if (scoreManager.GetPlayerName().IsNullOrWhitespace())
        {
            ShowLoginScreen();
        }
           
        cam = GameObject.Find("MainMenu").transform;  
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadSceneEnumerator(string levelName)
    {
        yield return new WaitForSeconds(0.5f);
        if (levelName == "this") SceneManager.LoadScene(SceneManager.loadedSceneCount);
        SceneManager.LoadScene(levelName);
    }
    
    public void LoadScene(string levelName) // For Resume, Next Level and Back to Main Menu
    {
        StartCoroutine(LoadSceneEnumerator(levelName));
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
        SoundManager.PlayOneShotSound(SoundManager.Sound.PlayerShieldOpen);

        while (elapsedTime < introSequenceTime)
        {
            cam.transform.localPosition = Vector3.Lerp(posStart, posGoal, (elapsedTime / introSequenceTime));
            cam.localScale = Vector3.Lerp(scaleStart, scaleGoal, (elapsedTime / introSequenceTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        LoadScene(level1Name);
    }

    
    public void LoginSceenSwitch()
    {
        scoreManager.SetPlayerName(usernameInput.GetComponent<TMPro.TMP_Text>().text);
        loginScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    public void ShowLoginScreen()
    {
        // TODO: Remove the current username. Potentially disconnect from Playfab.
        loginScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
    }
    public void PlayButtonSound(){
        SoundManager.PlayOneShotSound(SoundManager.Sound.ButtonSelect);
    }
    
    public void Close()
    {
        Application.Quit();
    }
}
