using System;
using System.Text;
using UnityEngine;

public class LoginFormPacketBuilder : IPacketBuilder
{
    private readonly string _username;
    private readonly string _password;

    public LoginFormPacketBuilder(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public byte[] Build()
    {
        byte[] packet = new byte[60];
        packet[0] = 0xC3;
        packet[1] = (byte)packet.Length;
        packet[2] = 0xF1;
        packet[3] = 0x01;

        // encrypt username with XOR3
        var userBytes = Xor3.Xor3Encrypt(_username, 10);
        Array.Copy(userBytes, 0, packet, 4, 10);

        // encrypt password with XOR3
        var passBytes = Xor3.Xor3Encrypt(_password, 20);
        Array.Copy(passBytes, 0, packet, 14, 20);

        //TickCount
        int tick = (int)(Time.realtimeSinceStartup * 1000);
        packet[34] = (byte)((tick >> 24) & 0xFF);
        packet[35] = (byte)((tick >> 16) & 0xFF);
        packet[36] = (byte)((tick >> 8) & 0xFF);
        packet[37] = (byte)(tick & 0xFF);

        // ClientVersion (5 bytes)
        byte[] clientVersion = GameState.Instance.CurrentHandshake.Version;

        string teste = Encoding.ASCII.GetString(clientVersion);

        Debug.Log($"[LoginFormPacketBuilder] clientVersion: {teste}");

        Array.Copy(clientVersion, 0, packet, 38, 5);

        // ClientSerial (16 bytes) — exemplo: "abcdefghijklmnop"
        byte[] serial = Encoding.ASCII.GetBytes("k1Pk2jcET48mxL3b");
        Array.Copy(serial, 0, packet, 43, 16);

        return packet;
    }
}