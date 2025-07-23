using System.Text;

public static class Xor3
{
    public static byte[] Xor3Encrypt(string input, int length)
    {
        byte[] data = Encoding.ASCII.GetBytes(input.PadRight(length, '\0'));
        byte[] key = { 0xFC, 0xCF, 0xAB };

        for (int i = 0; i < data.Length; i++)
            data[i] ^= key[i % 3];

        return data;
    }
}