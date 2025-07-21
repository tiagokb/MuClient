using System;

[Serializable]
public class FieldDefinition
{
    public int Index;
    public FieldType Type;
    public string Name;
    public int? Length; // For strings and arrays
    public string TypeName; // For structures (optional)
}

public enum FieldType
{
    Byte,
    Binary,
    String,
    StructureArray,
    Boolean,
    Enum,
    ShortLittleEndian,
    ShortBigEndian,
    LongBigEndian,
    LongLittleEndian,
    IntegerBigEndian,
    IntegerLittleEndian,
    Float,
}