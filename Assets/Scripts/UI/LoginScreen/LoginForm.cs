using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnLoginClicked()
    {
        GameEvents.OnLoginAttempt?.Invoke();

        string username = usernameInput.text;
        string password = passwordInput.text;

        PacketSender.SendAsync(new LoginFormPacketBuilder(username, password));
    }
}