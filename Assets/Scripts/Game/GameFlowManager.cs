using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        RegisterEventListeners();
    }

    private void RegisterEventListeners()
    {
        GameEvents.OnConnectionInfoReceived += OnConnectionInfoReceived;
        GameEvents.OnGameServerEntered += OnGameServerEntered;
    }

    private async void OnConnectionInfoReceived(GameServerInfo info)
    {
        await LoadingScreen.Instance.ShowStep("Conectando ao servidor de jogo...");
        await NetworkConnection.Instance.ConnectToServer(info.IpAddress, info.Port);
        await LoadingScreen.Instance.ShowStep("Aguardando resposta do servidor...");
        // Agora aguardamos o "Hello" do servidor, que deve acionar o próximo evento.
    }

    private async void OnGameServerEntered()
    {
        await LoadingScreen.Instance.ShowStep("Servidor pronto. Preparando login...");
        await Task.Delay(500); // só pra simular um carregamento suave
        GameEvents.OnGameServerEntered?.Invoke();
    }
}