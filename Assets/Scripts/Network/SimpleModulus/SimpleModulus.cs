using System;

public class SimpleModulus
{
    private readonly uint[] _modulusKey;
    private readonly uint[] _xorKey;
    private readonly uint[] _encryptKey;

    public SimpleModulus(uint[] modulusKey, uint[] xorKey, uint[] encryptKey)
    {
        _modulusKey = modulusKey;
        _xorKey = xorKey;
        _encryptKey = encryptKey;
    }

    public byte[] Encrypt(byte[] input)
    {
        ushort[] blocks = new ushort[4];
        for (int i = 0; i < 4 && i * 2 + 1 < input.Length; i++)
        {
            blocks[i] = BitConverter.ToUInt16(input, i * 2);
        }

        uint[] result = new uint[4];
        result[0] = ((_xorKey[0] ^ blocks[0]) * _encryptKey[0]) % _modulusKey[0];
        for (int i = 1; i < 4; i++)
        {
            result[i] = ((_xorKey[i] ^ (blocks[i] ^ (result[i - 1] & 0xFFFF))) * _encryptKey[i]) % _modulusKey[i];
        }

        for (int i = 0; i < 3; i++)
        {
            result[i] = result[i] ^ _xorKey[i] ^ (result[i + 1] & 0xFFFF);
        }

        // Compressão do resultado (similar ao server usar 11 bytes pra 6 uints)
        byte[] output = new byte[11];
        for (int i = 0; i < 4; i++)
        {
            int offset = i * 11 / 4;
            output[offset] = (byte)((result[i] >> 8) & 0xFF);
            if (offset + 1 < output.Length)
                output[offset + 1] = (byte)(result[i] & 0xFF);
        }

        return output;
    }
}