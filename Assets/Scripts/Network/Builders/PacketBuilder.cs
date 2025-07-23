using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PacketBuilder : IPacketBuilder
{
    private readonly PacketDefinition _definition;
    private readonly Dictionary<string, object> _fieldValues = new();

    public PacketBuilder(PacketDefinition definition)
    {
        _definition = definition;
    }

    public void SetField(string fieldName, object value)
    {
        _fieldValues[fieldName] = value;
    }

    public byte[] Build()
    {
        var length = _definition.Length ?? CalculateDynamicLength();
        var buffer = new byte[length];
        int offset = GetHeaderLength(_definition.HeaderType);

        // Header
        buffer[0] = GetHeaderByte(_definition.HeaderType);
        buffer[1] = (byte)length;
        buffer[2] = _definition.Code;
        if (_definition.SubCode.HasValue)
            buffer[3] = _definition.SubCode.Value;

        foreach (var field in _definition.Fields)
        {
            var value = _fieldValues[field.Name];
            WriteField(buffer, field, value);
        }

        return buffer;
    }

    private void WriteField(byte[] buffer, FieldDefinition field, object value)
    {
        switch (field.Type)
        {
            case FieldType.Byte:
                buffer[field.Index] = Convert.ToByte(value);
                break;
            case FieldType.ShortLittleEndian:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.ShortBigEndian:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.String:
                var stringBytes = Encoding.UTF8.GetBytes(value.ToString());
                Array.Copy(stringBytes, 0, buffer, field.Index, field.Length ?? stringBytes.Length);
                break;
            case FieldType.StructureArray:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.Boolean:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.Binary:
                var binaryBytes = (byte[])value;
                Array.Copy(binaryBytes, 0, buffer, field.Index, field.Length ?? binaryBytes.Length);
                break;
            case FieldType.Enum:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.IntegerBigEndian:
                uint convertedIntegerBigEndianValue = Convert.ToUInt32(value);
                buffer[field.Index] = (byte)((convertedIntegerBigEndianValue >> 24) & 0xFF);
                buffer[field.Index + 1] = (byte)((convertedIntegerBigEndianValue >> 16) & 0xFF);
                buffer[field.Index + 2] = (byte)((convertedIntegerBigEndianValue >> 8) & 0xFF);
                buffer[field.Index + 3] = (byte)(convertedIntegerBigEndianValue & 0xFF);
                break;
            case FieldType.IntegerLittleEndian:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.LongLittleEndian:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.LongBigEndian:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            case FieldType.Float:
                //TODO: Implement type writer
                throw new Exception($"Unknown header type {field.Type}");
                break;
            // adicione os outros tipos conforme necessário
            default:
                throw new Exception($"Unknown header type {field.Type}");
                break;
        }
    }

    private int GetHeaderLength(string headerType) =>
        headerType switch
        {
            "C1" => 2,
            "C3" => 2,
            "C1HeaderWithSubCode" => 2,
            "C3HeaderWithSubCode" => 2,

            "C2" => 3,
            "C4" => 3,

            _ => throw new Exception($"Unknown header type {headerType}")
        };

    private byte GetHeaderByte(string headerType) =>
        headerType switch
        {
            "C1" => 0xC1,
            "C1HeaderWithSubCode" => 0xC1,
            "C2" => 0xC2,
            "C3" => 0xC3,
            "C3HeaderWithSubCode" => 0xC3,
            "C4" => 0xC3,

            _ => throw new Exception($"Unknown header type {headerType}")
        };

    private int CalculateDynamicLength()
    {
        if (_definition.Fields == null || !_definition.Fields.Any())
            throw new InvalidOperationException("No fields defined for this packet.");

        var lastField = _definition.Fields
            .OrderByDescending(f => f.Index)
            .First();

        return lastField.Index + (lastField.Length ?? 1) + GetHeaderLength(_definition.HeaderType);
    }
}