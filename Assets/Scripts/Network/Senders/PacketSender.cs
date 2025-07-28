using System;
using System.Threading.Tasks;
using UnityEngine;

public static class PacketSender
{
    public static void Send(IPacketBuilder builder)
    {
        byte[] packet = builder.Build();
        NetworkConnection.Instance.EnqueueSend(packet);
    }

    public static void SendAsync(IAsyncPacketBuilder builder)
    {
        Task.Run(async () =>
        {
            var packet = await builder.BuildeAsync().ConfigureAwait(false);
            NetworkConnection.Instance.EnqueueSend(packet);
        });
    }
}