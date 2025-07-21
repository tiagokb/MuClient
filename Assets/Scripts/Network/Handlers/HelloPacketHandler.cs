using UnityEngine;

public class HelloPacketHandler : IPacketHandler
{
    public byte Code => 0x00;
    public byte? SubCode => 0x01;

    public void Handle(ParsedPacket packet)
    {
        Debug.Log("[HelloPacketHandler] Servidor disse 'Olá'. Vamos pedir a lista de servidores!");
        // aqui depois a gente chama o sender pra mandar o próximo pacote
        PacketSender.SendAsync(new RequestServerList());
    }
}