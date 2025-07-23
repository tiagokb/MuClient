using System;
using UnityEngine;

public static class PacketSender
{
    public static async void SendAsync(IPacketBuilder builder)
    {
        byte[] packet = builder.Build();

        Debug.Log($"[SendAsync] Enviando pacote para o servidor: {BitConverter.ToString(packet)}");
        await NetworkConnection.Instance.SendAsync(packet);
    }
}