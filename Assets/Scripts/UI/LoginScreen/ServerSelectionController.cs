using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelectionController : MonoBehaviour
{
    [SerializeField] private Transform serverButtonContainer;
    [SerializeField] private GameObject serverButtonPrefab;

    private void Start()
    {
        var servers = GameState.Instance.AvailableServers;

        if (servers == null || servers.Count == 0)
        {
            Debug.LogWarning("[ServerSelectionController] Nenhum servidor disponível.");
            return;
        }

        foreach (var server in servers)
        {
            CreateServerButton(server);
        }
    }

    private void CreateServerButton(ServerInfo server)
    {
        var buttonGo = Instantiate(serverButtonPrefab, serverButtonContainer);
        var buttonText = buttonGo.GetComponentInChildren<TMP_Text>();
        buttonText.text = $"Server {server.Id} - Load: {server.LoadPercentage}%";

        var button = buttonGo.GetComponent<Button>();
        button.onClick.AddListener(() => OnSelectServer(server));
    }

    private void OnSelectServer(ServerInfo server)
    {
        Debug.Log($"[ServerSelectionController] Servidor selecionado: {server.Id}");
        PacketSender.Send(new SelectServer(server.Id));
        LoginScreenUIRoot.Instance.HideUI();
    }
}