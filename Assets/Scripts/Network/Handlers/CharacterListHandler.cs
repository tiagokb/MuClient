using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListHandler : IPacketHandler
{
    public byte Code => 0xF3;
    public byte? SubCode => 0x00;

    public void Handle(ParsedPacket packet)
    {
        var packetDictionary = packet.Decode(PacketDefinitionsRegistry.PacketType.ServerToClient);

        Debug.Log($"CharacterList received: {FormatPacketDictionary(packetDictionary)}");
    }

    public static string FormatPacketDictionary(object obj, int indentLevel = 0)
    {
        var indent = new string(' ', indentLevel * 2);
        var builder = new System.Text.StringBuilder();

        switch (obj)
        {
            case Dictionary<string, object> dict:
                foreach (var kv in dict)
                {
                    builder.Append(indent + kv.Key + ": ");
                    if (kv.Value is Dictionary<string, object> || kv.Value is IEnumerable<object>)
                    {
                        builder.AppendLine();
                        builder.Append(FormatPacketDictionary(kv.Value, indentLevel + 1));
                    }
                    else
                    {
                        builder.AppendLine(kv.Value?.ToString() ?? "null");
                    }
                }

                break;

            case IEnumerable<object> list:
                int index = 0;
                foreach (var item in list)
                {
                    builder.AppendLine($"{indent}- [{index}]");
                    builder.Append(FormatPacketDictionary(item, indentLevel + 1));
                    index++;
                }

                break;

            default:
                builder.AppendLine(indent + (obj?.ToString() ?? "null"));
                break;
        }

        return builder.ToString();
    }
}