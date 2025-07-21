using System;
using UnityEngine;

public class ConnectionInfo : IPacketHandler
{
    public byte Code => 0xF4;
    public byte? SubCode => 0x03;

    public void Handle(ParsedPacket packet)
    {
        var definition = packet.Decode(PacketDefinitionsRegistry.PacketType.Connect);

        GameServerInfo gameServerInfo = new GameServerInfo
        {
            IpAddress = definition["IpAddress"] as string,
            Port = Convert.ToInt32(definition["Port"]),
        };

        GameEvents.OnConnectionInfoReceived?.Invoke(gameServerInfo);
    }
}