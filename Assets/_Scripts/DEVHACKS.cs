
using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class DEVHACKS : MonoBehaviour
{
    [SerializeField] private string SceneToRestartFrom;

    public static event Action OnForceRestart;
    
    public DEVHACKS Instance { get; set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }
    
    private void RestartScene()
    { 
        OnForceRestart?.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneToRestartFrom);
    }
}
