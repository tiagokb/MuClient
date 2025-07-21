public static class PacketSender
{
    public static async void SendAsync(IPacketBuilder builder)
    {
        await NetworkConnection.Instance.SendAsync(builder.Build());
    }
}