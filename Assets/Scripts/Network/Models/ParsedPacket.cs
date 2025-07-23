using System;
using System.Collections.Generic;
using System.Linq;
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
            if (field.Type == FieldType.StructureArray)
                continue;

            if (field.Name == "CharacterCount")
            {
                Debug.Log(
                    $"[Decode] Index: {field.Index} Name: {field.Name} Type: {field.Type} PacketValue: {Packet[field.Index]}");
            }

            object value = field.Type switch
            {
                _ => ReadFieldValue(field, Packet, field.Index)
            };

            result[field.Name] = value;
        }

        foreach (var field in definition.Fields)
        {
            if (field.Type != FieldType.StructureArray)
                continue;

            var value = ReadStructureArray(Packet, field, definition, result);
            result[field.Name] = value;
        }

        return result;
    }

    public override string ToString()
    {
        return
            $"Header: {Header:X2}, Code: {Code:X2}, SubCode: {SubCode?.ToString("X2") ?? "N/A"}, Payload: {BitConverter.ToString(Payload)}";
    }

    private static string ReadString(byte[] data, int index, int? length)
    {
        int l = length ?? data.Length;

        var bytes = new byte[l];
        Array.Copy(data, index, bytes, 0, l);
        return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
    }

    private static byte[] ReadBytes(byte[] data, int index, int? length)
    {
        int l = length ?? data.Length;

        var result = new byte[l];
        Array.Copy(data, index, result, 0, l);
        return result;
    }

    private static object ReadStructureArray(byte[] payload, FieldDefinition field, PacketDefinition definition,
        Dictionary<string, object> decodedFields)
    {
        var structures = new List<Dictionary<string, object>>();

        var structureDef = definition.Structures.Find(s => s.Name == field.TypeName);
        if (structureDef == null)
            throw new Exception($"Estrutura '{field.TypeName}' não encontrada. field: {field.Name}");

        int structureLength = structureDef.Length ?? CalculateStructureLength(structureDef);

        int count;
        if (!string.IsNullOrEmpty(field.ItemCountField))
        {
            if (!decodedFields.TryGetValue(field.ItemCountField, out var countObj))
                throw new Exception($"Campo de contagem '{field.ItemCountField}' não foi decodificado antes.");

            count = Convert.ToInt32(countObj); // Pode ser byte, int, etc.

            Debug.Log(
                $"[ReadStructureArray] o count ta sendo processado: {count}, valor anterior: {countObj}");
        }
        else
        {
            Debug.Log($"[ReadStructureArray] o count ta caindo em fallback");
            // fallback: tenta calcular pelo tamanho
            count = (payload.Length - field.Index) / structureLength;

            Debug.Log($"[ReadStructureArray] o count ta caindo em fallback: {count}");
        }

        for (int i = 0; i < count; i++)
        {
            var structure = new Dictionary<string, object>();
            int baseOffset = field.Index + i * structureLength;

            foreach (var f in structureDef.Fields)
            {
                int offset = baseOffset + f.Index;

                object val = ReadFieldValue(f, payload, offset);

                structure[f.Name] = val;
            }

            structures.Add(structure);
        }

        return structures;
    }

    private static object ReadFieldValue(FieldDefinition field, byte[] data, int offset)
    {
        return field.Type switch
        {
            FieldType.Byte => data[offset],
            FieldType.Boolean => data[offset] != 0,

            FieldType.ShortLittleEndian => BitConverter.ToUInt16(data, offset),
            FieldType.ShortBigEndian => (ushort)((data[offset] << 8) | data[offset + 1]),

            FieldType.IntegerLittleEndian => BitConverter.ToInt32(data, offset),
            FieldType.IntegerBigEndian => (data[offset] << 24) | (data[offset + 1] << 16)
                                                               | (data[offset + 2] << 8) | data[offset + 3],

            FieldType.LongLittleEndian => BitConverter.ToInt64(data, offset),
            FieldType.LongBigEndian => ((long)data[offset] << 56) | ((long)data[offset + 1] << 48)
                                                                  | ((long)data[offset + 2] << 40) |
                                                                  ((long)data[offset + 3] << 32)
                                                                  | ((long)data[offset + 4] << 24) |
                                                                  ((long)data[offset + 5] << 16)
                                                                  | ((long)data[offset + 6] << 8) | data[offset + 7],

            FieldType.Float => BitConverter.ToSingle(data, offset),

            FieldType.String => ReadString(data, offset, field.Length),
            FieldType.Binary => ReadBytes(data, offset, field.Length),

            FieldType.Enum => data[offset],

            _ => throw new NotSupportedException($"Tipo de campo {field.Type} não suportado."),
        };
    }

    private static int CalculateStructureLength(StructureDefinition def)
    {
        var lastField = def.Fields
            .OrderByDescending(f => f.Index + (f.Length ?? 1))
            .FirstOrDefault();

        return (lastField?.Index ?? 0) + (lastField?.Length ?? 1);
    }
}