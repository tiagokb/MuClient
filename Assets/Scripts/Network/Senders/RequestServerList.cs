public class RequestServerList : IPacketBuilder
{
    public byte[] Build()
    {
        // C1 04 F4 06
        return new byte[] { 0xC1, 0x04, 0xF4, 0x06 };
    }
}