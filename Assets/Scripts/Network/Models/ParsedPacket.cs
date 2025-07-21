using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ParsedPacket
{
    public byte Header;
    public byte Code;
    public byte? SubCode;
    public byte[] Payload;
    public byte[] Packet;

    public Dictionary<string, object> Decode(PacketDefinitionsRegistry.PacketType packetType)
    {
        var definition = PacketDefinitionsRegistry.Instance.GetDefinition(this.Code, this.SubCode, packetType);

        var result = new Dictionary<string, object>();

        foreach (var field in definition.Fields)
        {
            Debug.Log($"[ParsedPacket] decoding field name: {field.Name}");
            Debug.Log($"[ParsedPacket] decoding field index: {field.Index}");
            Debug.Log($"[ParsedPacket] decoding field type: {field.Type}");
            Debug.Log($"[ParsedPacket] decoding field length {field.Length}");

            Debug.Log($"[ParsedPacket] decoding payload {field.Length}");

            object value = field.Type switch
            {
                FieldType.Byte => Packet[field.Index],
                FieldType.ShortLittleEndian => BitConverter.ToUInt16(Packet, field.Index),
                FieldType.ShortBigEndian => (ushort)((Packet[field.Index] << 8) | Packet[field.Index + 1]),
                FieldType.String => ReadString(Packet, field.Index, field.Length),
                FieldType.StructureArray => ReadStructureArray(Packet, field, definition),
                _ => throw new NotSupportedException($"Tipo de campo {field.Type} não suportado.")
            };

            Debug.Log($"[ParsedPacket] decoded {value}");

            result[field.Name] = value;
        }

        return result;
    }

    public override string ToString()
    {
        return
            $"Header: {Header:X2}, Code: {Code:X2}, SubCode: {SubCode?.ToString("X2") ?? "N/A"}, Payload: {BitConverter.ToString(Payload)}";
    }

    private static string ReadString(byte[] payload, int index, int? length)
    {
        int l = length ?? payload.Length;

        var bytes = new byte[l];
        Array.Copy(payload, index, bytes, 0, l);
        return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
    }

    private static object ReadStructureArray(byte[] payload, FieldDefinition field, PacketDefinition definition)
    {
        var structures = new List<Dictionary<string, object>>();

        var structureDef = definition.Structures.Find(s => s.Name == field.TypeName);
        if (structureDef == null)
            throw new Exception($"Estrutura '{field.TypeName}' não encontrada.");

        int count = (payload.Length - field.Index) / structureDef.Length;
        for (int i = 0; i < count; i++)
        {
            var structure = new Dictionary<string, object>();
            int baseOffset = field.Index + i * structureDef.Length;

            foreach (var f in structureDef.Fields)
            {
                object val = f.Type switch
                {
                    FieldType.Byte => payload[baseOffset + f.Index],
                    FieldType.ShortLittleEndian => BitConverter.ToUInt16(payload, baseOffset + f.Index),
                    _ => throw new NotSupportedException($"Tipo {f.Type} em estrutura não suportado.")
                };

                structure[f.Name] = val;
            }

            structures.Add(structure);
        }

        return structures;
    }
}