public interface IPacketHandler
{
    byte Code { get; }
    byte? SubCode { get; }
    void Handle(ParsedPacket packet);
}