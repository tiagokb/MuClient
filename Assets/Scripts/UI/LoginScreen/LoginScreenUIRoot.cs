using System;
using UnityEngine;

public class LoginScreenUIRoot : MonoBehaviour
{
    public static LoginScreenUIRoot Instance { get; private set; }

    [SerializeField] private GameObject serverListPanel;
    [SerializeField] private GameObject loginPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GameEvents.OnGameServerEntered += HandleGameServerEntered;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameServerEntered -= HandleGameServerEntered;
    }

    private void HandleGameServerEntered()
    {
        ShowLoginForm();
    }

    public void ShowServerList()
    {
        serverListPanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    public void ShowLoginForm()
    {
        serverListPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void HideUI()
    {
        serverListPanel.SetActive(false);
        loginPanel.SetActive(false);
    }
}