using System;
using System.Collections.Generic;

using URFCommon;

/// <summary>
/// Defines basic meta information about entities.
/// </summary>
public class EntityInfoSystem : BaseRulesSystem
{
    public override List<Type> Components => new () {
        // todo could create an annotation to register these
        typeof(EntityInfo)
    };
}

[Component("aa058eef-1ca6-401f-b16c-81811e93a40a")]
public class EntityInfo : BaseComponent
{
    public string Name {get;set;}
    public string Appearance {get;set;}
    public override void Load(GameDataReader reader)
    {
        Name = reader.ReadString();
        Appearance = reader.ReadString();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Name);
        writer.Write(Appearance);
    }
}


