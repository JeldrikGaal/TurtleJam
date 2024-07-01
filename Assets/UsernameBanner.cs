using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UsernameBanner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;


    private void Start()
    {
        if (usernameText == null)
            usernameText = GameObject.FindWithTag("UsernameBanner").GetComponent<TMPro.TextMeshProUGUI>();

    }

    public void DisplayUsernameInBanner(string username)
    {

        if (usernameText == null)
            usernameText = GameObject.FindWithTag("UsernameBanner").GetComponent<TMPro.TextMeshProUGUI>();
        usernameText.text = username;
    }
}
