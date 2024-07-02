using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoginVisuals : MonoBehaviour
{
    [SerializeField] private TMP_InputField _userNameTextField;
    [SerializeField] private GameObject _inputFieldObject;
    [SerializeField] private GameObject _loginButtonObject;
    [SerializeField] private Image _inputFieldImage;
    [SerializeField] private Color _correctBlinkColor;

    public static event Action LoginVisualsDone;
    
    private List<string> _characters = new List<string>() 
    { 
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", 
        "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", 
        "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
    };

    private void Awake()
    {
        SceneLoader.OnLoginButton += LoginAnim;
    }

    private void OnDestroy()
    {
        SceneLoader.OnLoginButton -= LoginAnim;
    }

    private void LoginAnim()
    {
        _loginButtonObject.transform.DOScale(Vector3.zero, 0.5f);
        StartCoroutine(ScrambleLetters(1f, 0.1f));
    }

    private IEnumerator ScrambleLetters(float duration, float timePerSwitch)
    {
        float timeElapsed = 0;
        int nameLength = _userNameTextField.text.Length;
        string originalName = _userNameTextField.text;
        while (timeElapsed < duration)
        {
            _userNameTextField.text = "";
            for (int i = 0; i < nameLength; i++)
            {
                _userNameTextField.text += _characters[Random.Range(0, _characters.Count - 1)];
            }
            timeElapsed += timePerSwitch;
            yield return new WaitForSeconds(timePerSwitch);
        }

        _userNameTextField.text = originalName;
        float blinkWaitTime = 0.3f;
        
        _inputFieldImage.color = _correctBlinkColor;
        yield return new WaitForSeconds(blinkWaitTime);
        _inputFieldImage.color = Color.white;
        yield return new WaitForSeconds(blinkWaitTime);
        _inputFieldImage.color = _correctBlinkColor;
        yield return new WaitForSeconds(blinkWaitTime);
        _inputFieldImage.color = Color.white;
        yield return new WaitForSeconds(blinkWaitTime);
        _inputFieldImage.color = _correctBlinkColor;
        yield return new WaitForSeconds(blinkWaitTime);
        _inputFieldImage.color = Color.white;
        yield return new WaitForSeconds(blinkWaitTime);
        
        LoginVisualsDone?.Invoke();
        
        

        //_inputFieldObject.transform.DOScale(Vector3.zero, 0.5f);
    }
}
