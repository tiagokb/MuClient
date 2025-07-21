using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RequestServerListHandler : IPacketHandler
{
    public byte Code => 0xF4;
    public byte? SubCode => 0x06;

    public void Handle(ParsedPacket packet)
    {
        if (packet.Payload.Length < 2)
        {
            Debug.LogWarning("[ServerListResponseHandler] Payload muito curto!");
            return;
        }

        // Leitura do ServerCount (Short Big Endian)
        ushort serverCount = (ushort)((packet.Payload[0] << 8) + packet.Payload[1]);
        Debug.Log($"[ServerListResponseHandler] Servidores disponíveis: {serverCount}");

        List<ServerInfo> servers = new();

        for (int i = 0; i < serverCount; i++)
        {
            int offset = 2 + i * 4;

            if (packet.Payload.Length < offset + 4)
            {
                Debug.LogWarning($"[ServerListResponseHandler] Dados insuficientes para o servidor {i}.");
                continue;
            }

            ushort serverId = (ushort)(packet.Payload[offset] | packet.Payload[offset + 1] << 8); // little endian
            byte load = packet.Payload[offset + 2];

            servers.Add(new ServerInfo { Id = serverId, LoadPercentage = load });
        }

        foreach (var server in servers)
        {
            Debug.Log($"[Servidor] ID: {server.Id}, Carga: {server.LoadPercentage}%");
        }

        GameState.Instance.AvailableServers = servers;
        LoadingScreen.Instance.Hide();
        SceneManager.LoadScene("LoginScreen");
    }
}