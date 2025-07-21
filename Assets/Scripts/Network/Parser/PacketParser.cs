using System;
using UnityEngine;

public class PacketParser
{
    public ParsedPacket Parse(byte[] rawPacket)
    {
        if (rawPacket == null || rawPacket.Length < 3)
            throw new ArgumentException("Pacote invalido ou muito curto!");

        var header = rawPacket[0];
        int length = GetPacketLength(header, rawPacket);

        if (length != rawPacket.Length)
            Debug.LogWarning(
                $"[PacketParser] O tamanho do pacote ({rawPacket.Length}) não bate com o header ({length})");

        byte code;
        byte? subCode = null;
        int offset;

        switch (header)
        {
            case 0xC1:
            case 0xC3:
                code = rawPacket[2];
                offset = 3;

                if (HasSubCode(code))
                {
                    subCode = rawPacket[3];
                    offset++;
                }

                break;

            case 0xC2:
            case 0xC4:
                code = rawPacket[3];
                offset = 4;
                if (HasSubCode(code))
                {
                    subCode = rawPacket[4];
                    offset++;
                }

                break;
            default:
                throw new NotSupportedException($"Header {header:X2} ainda não suportado.");
        }

        int payloadLength = rawPacket.Length - offset;
        byte[] payload = new byte[payloadLength];
        Array.Copy(rawPacket, offset, payload, 0, payloadLength);

        return new ParsedPacket
        {
            Header = header,
            Code = code,
            SubCode = subCode,
            Payload = payload,
            Packet = rawPacket,
        };
    }

    private int GetPacketLength(byte header, byte[] rawPacket)
    {
        return header switch
        {
            0xC1 or 0xC3 => rawPacket[1],
            0xC2 or 0xC4 => (rawPacket[1] << 8) | rawPacket[2],
            _ => throw new NotSupportedException($"Header {header:X2} ainda não suportado para cálculo de tamanho!"),
        };
    }

    private bool HasSubCode(byte code)
    {
        return code == 0xF1 || code == 0x00 || code == 0xF4;
    }
}