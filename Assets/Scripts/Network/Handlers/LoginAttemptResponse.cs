using System;
using System.Linq;
using UnityEngine;

public class LoginAttemptResponse : IPacketHandler
{
    public byte Code => 0xF1;
    public byte? SubCode => 0x01;

    public void Handle(ParsedPacket packet)
    {
        var packetRef = packet.Decode(PacketDefinitionsRegistry.PacketType.ServerToClient);

        LoginAttemptResponseEnum result = Enum.IsDefined(typeof(LoginAttemptResponseEnum), packetRef["Success"])
            ? (LoginAttemptResponseEnum)packetRef["Success"]
            : LoginAttemptResponseEnum.UnkownError;
        
        GameEvents.OnLoginAttemptResponse?.Invoke(result);

        Debug.Log($"[LoginAttemptResponse] type: {result}");
    }
}