using UnityEngine;

public class GameServerHandshake : IPacketHandler
{
    public byte Code => 0xF1;
    public byte? SubCode => 0x00;

    public void Handle(ParsedPacket packet)
    {
        var packetDictionary = packet.Decode(PacketDefinitionsRegistry.PacketType.ServerToClient);

        HandshakeInfo handshakeInfo = new HandshakeInfo
        {
            PlayerId = (ushort)packetDictionary["PlayerId"],
            Version = packetDictionary["Version"] as byte[],
            VersionString = packetDictionary["VersionString"] as string,
        };

        GameState.Instance.CurrentHandshake = handshakeInfo;
        GameEvents.OnGameServerEntered?.Invoke();
    }
}