public class DeleteCharacter
{
    private const byte Code = 0xF3;
    private const byte SubCode = 0x02;

    private readonly string _name;
    private readonly string _securityCode;

    public DeleteCharacter(string name, string securityCode)
    {
        _name = name;
        _securityCode = securityCode;
    }

    public void SendPacket()
    {
        var def = PacketDefinitionsRegistry.Instance.GetDefinition(Code, SubCode,
            PacketDefinitionsRegistry.PacketType.ClientToServer);

        var packetBuilder = new PacketBuilder(def);

        packetBuilder.SetField("Name", _name);
        packetBuilder.SetField("SecurityCode", _securityCode);

        PacketSender.Send(packetBuilder);
    }
}