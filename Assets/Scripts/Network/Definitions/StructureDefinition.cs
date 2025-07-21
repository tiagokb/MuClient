using System;
using System.Collections.Generic;

[Serializable]
public class StructureDefinition
{
    public string Name;
    public int Length;
    public List<FieldDefinition> Fields = new();
}