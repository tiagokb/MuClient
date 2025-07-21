// Assets/Scripts/Network/Definitions/PacketDefinition.cs

using System;
using System.Collections.Generic;

[Serializable]
public class PacketDefinition
{
    public string Name;
    public string HeaderType;
    public byte Code;
    public byte? SubCode;
    public string Direction;
    public int? Length; // For strings and arrays
    public List<FieldDefinition> Fields = new();
    public List<StructureDefinition> Structures = new();
}