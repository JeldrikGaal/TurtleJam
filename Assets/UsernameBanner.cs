using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UsernameBanner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI usernameText;

    public void DisplayUsernameInBanner(string username)
    {
        usernameText.text = username;
    }
}
