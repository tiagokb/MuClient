using UnityEngine;

public class CharacterDeleteResponse : IPacketHandler
{
    public byte Code => 0xF3;
    public byte? SubCode => 0x02;

    public void Handle(ParsedPacket packet)
    {
        var packetDictionary = packet.Decode(PacketDefinitionsRegistry.PacketType.ServerToClient);
        var charactersRaw = (CharacterDeleteResult)packetDictionary["Result"];
        CharacterSelectionEvents.OnCharacterDeleteStatusReturn?.Invoke(charactersRaw);
        Debug.Log($"[CharacterDeleteResponse] response to delete a character: {charactersRaw}");
    }
}