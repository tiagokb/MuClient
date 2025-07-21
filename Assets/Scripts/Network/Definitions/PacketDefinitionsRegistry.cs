using System;
using System.Collections.Generic;

public class PacketDefinitionsRegistry
{
    public enum PacketType
    {
        Connect,
        ServerToClient,
        ClientToServer
    }

    private static PacketDefinitionsRegistry _instance;
    public static PacketDefinitionsRegistry Instance => _instance ??= new PacketDefinitionsRegistry();

    private Dictionary<(byte, byte?), PacketDefinition> _connectServerDefinitions;
    private Dictionary<(byte, byte?), PacketDefinition> _serverToClientDefinitions;
    private Dictionary<(byte, byte?), PacketDefinition> _clientToServerDefinitions;

    public void Load()
    {
        _connectServerDefinitions = PacketDefinitionLoader.Load(PacketType.Connect);
        _serverToClientDefinitions = PacketDefinitionLoader.Load(PacketType.ServerToClient);
        _clientToServerDefinitions = PacketDefinitionLoader.Load(PacketType.ClientToServer);
    }

    public PacketDefinition GetDefinition(byte code, byte? subCode, PacketType packetType)
    {
        switch (packetType)
        {
            case PacketType.Connect:
                _connectServerDefinitions.TryGetValue((code, subCode), out var cs);
                return cs;
            case PacketType.ServerToClient:
                _serverToClientDefinitions.TryGetValue((code, subCode), out var stc);
                return stc;
            case PacketType.ClientToServer:
                _clientToServerDefinitions.TryGetValue((code, subCode), out var cts);
                return cts;
            default: throw new Exception("Unknown packet type");
        }
    }

    public bool TryGet(byte code, byte? subCode, PacketType packetType, out PacketDefinition definition)
    {
        switch (packetType)
        {
            case PacketType.Connect:
                return _connectServerDefinitions.TryGetValue((code, subCode), out definition);
            case PacketType.ServerToClient:
            case PacketType.ClientToServer:
            default: throw new Exception("Unknown packet type");
        }
    }
}