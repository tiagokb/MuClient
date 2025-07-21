using System.Collections.Generic;
using UnityEngine;

public class PacketRouter
{
    private readonly Dictionary<(byte code, byte? subCode), IPacketHandler> _handlers = new();

    public void RegisterHandler(IPacketHandler handler)
    {
        var key = (handler.Code, handler.SubCode);

        if (_handlers.ContainsKey(key))
        {
            Debug.Log($"[PacketRouter] Handler duplicado para codigo {handler.Code:X2} subcódigo {handler.SubCode:X2}");
            return;
        }

        _handlers[key] = handler;
    }

    public void Route(ParsedPacket packet)
    {
        var key = (packet.Code, packet.SubCode);
        if (_handlers.TryGetValue(key, out var handler))
        {
            handler.Handle(packet);
        }
        else
        {
            Debug.LogWarning(
                $"[PacketRouter] Nenhum handler registrado para Code: {packet.Code:X2}, SubCode: {packet.SubCode:X2}");
        }
    }
}