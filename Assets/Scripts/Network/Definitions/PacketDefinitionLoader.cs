using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public static class PacketDefinitionLoader
{
    public static Dictionary<(byte, byte?), PacketDefinition> Load(
        PacketDefinitionsRegistry.PacketType packetType)
    {
        TextAsset xmlAsset = null;

        switch (packetType)
        {
            case PacketDefinitionsRegistry.PacketType.Connect:
                xmlAsset = Resources.Load<TextAsset>("connect_server_packet_definitions");
                break;
            case PacketDefinitionsRegistry.PacketType.ServerToClient:
                xmlAsset = Resources.Load<TextAsset>("server_to_client_packet_definitions");
                break;
            case PacketDefinitionsRegistry.PacketType.ClientToServer:
                xmlAsset = Resources.Load<TextAsset>("client_to_server_packet_definitions");
                break;
            default:
                xmlAsset = null;
                break;
        }

        if (xmlAsset == null)
        {
            Debug.LogError(
                "[PacketDefinitionLoader] Não foi possível carregar o arquivo XML de definições de connect_server.");
            return new Dictionary<(byte, byte?), PacketDefinition>();
        }

        XDocument doc = XDocument.Parse(xmlAsset.text);
        XNamespace ns = "http://www.munique.net/OpenMU/PacketDefinitions";

        var packetElements = doc.Root?.Element(ns + "Packets")?.Elements(ns + "Packet");
        if (packetElements == null)
        {
            Debug.LogError("[PacketDefinitionLoader] Nenhum pacote encontrado no XML de connect_server.");
            return new Dictionary<(byte, byte?), PacketDefinition>();
        }

        var definitions = new Dictionary<(byte, byte?), PacketDefinition>();

        foreach (var element in packetElements)
        {
            byte code = Convert.ToByte(element.Element(ns + "Code")?.Value ?? "0", 16);
            byte? subCode = null;

            var subCodeElement = element.Element(ns + "SubCode");
            if (subCodeElement != null)
                subCode = Convert.ToByte(subCodeElement.Value, 16);

            var definition = new PacketDefinition
            {
                HeaderType = element.Element(ns + "HeaderType")?.Value,
                Code = code,
                SubCode = subCode,
                Name = element.Element(ns + "Name")?.Value,
                Direction = element.Element(ns + "Direction")?.Value,
                Length = int.TryParse(element.Element(ns + "Length")?.Value, out var len) ? len : (int?)null,
                Fields = element.Element(ns + "Fields")?.Elements(ns + "Field").Select(f => new FieldDefinition
                {
                    Index = int.Parse(f.Element(ns + "Index")?.Value ?? "0"),
                    Name = f.Element(ns + "Name")?.Value,
                    Type = TryMapFieldType(f.Element(ns + "Type")?.Value, out var ft)
                        ? ft
                        : throw new Exception($"Tipo inválido: {f.Element(ns + "Type")?.Value}"),
                    Length = int.TryParse(f.Element(ns + "Length")?.Value, out var flen) ? flen : (int?)null,
                    TypeName = f.Element(ns + "TypeName")?.Value,
                    ItemCountField = f.Element(ns + "ItemCountField")?.Value,
                }).ToList(),
                Structures = element.Element(ns + "Structures")?.Elements(ns + "Structure").Select(f =>
                    new StructureDefinition
                    {
                        Name = f.Element(ns + "Name")?.Value,
                        Length = int.TryParse(f.Element(ns + "Length")?.Value, out var len2) ? len2 : (int?)null,
                        Fields = f.Element(ns + "Fields")?.Elements(ns + "Field").Select(f2 => new FieldDefinition
                        {
                            Index = int.Parse(f2.Element(ns + "Index")?.Value ?? "0"),
                            Name = f2.Element(ns + "Name")?.Value,
                            Type = TryMapFieldType(f2.Element(ns + "Type")?.Value, out var ft)
                                ? ft
                                : throw new Exception($"Tipo inválido: {f2.Element(ns + "Type")?.Value}"),
                            Length = int.TryParse(f2.Element(ns + "Length")?.Value, out var flen3) ? flen3 : (int?)null,
                            TypeName = f2.Element(ns + "TypeName")?.Value,
                            ItemCountField = f.Element(ns + "ItemCountField")?.Value,
                        }).ToList(),
                    }).ToList(),
            };

            definitions[(code, subCode)] = definition;
        }

        Debug.Log($"[PacketDefinitionLoader] {definitions.Count} definições de pacotes carregadas.");
        return definitions;
    }

    private static bool TryMapFieldType(string typeString, out FieldType fieldType)
    {
        switch (typeString)
        {
            case "Byte":
                fieldType = FieldType.Byte;
                return true;
            case "ShortLittleEndian":
                fieldType = FieldType.ShortLittleEndian;
                return true;
            case "ShortBigEndian":
                fieldType = FieldType.ShortBigEndian;
                return true;
            case "String":
                fieldType = FieldType.String;
                return true;
            case "Structure[]":
                fieldType = FieldType.StructureArray;
                return true;
            case "Boolean":
                fieldType = FieldType.Boolean;
                return true;
            case "Binary":
                fieldType = FieldType.Binary;
                return true;
            case "Enum":
                fieldType = FieldType.Enum;
                return true;
            case "IntegerBigEndian":
                fieldType = FieldType.IntegerBigEndian;
                return true;
            case "IntegerLittleEndian":
                fieldType = FieldType.IntegerLittleEndian;
                return true;
            case "LongLittleEndian":
                fieldType = FieldType.LongLittleEndian;
                return true;
            case "LongBigEndian":
                fieldType = FieldType.LongBigEndian;
                return true;
            case "Float":
                fieldType = FieldType.Float;
                return true;
            // adicione os outros tipos conforme necessário
            default:
                fieldType = default;
                return false;
        }
    }
}