public class SelectServer : IPacketBuilder
{
    private readonly ushort _serverId;

    public SelectServer(ushort serverId)
    {
        _serverId = serverId;
    }

    public byte[] Build()
    {
        return new byte[] { 0xC1, 0x06, 0xF4, 0x03, (byte)(_serverId & 0xFF), (byte)((_serverId >> 8) & 0xFF) };
    }
}