using System.Collections.Generic;

using URFCommon;

/// <summary>
/// Defines basic meta information about entities.
/// </summary>
public class EntityInfoSystem : BaseRulesSystem
{
    public override List<(string, SlotType)> Slots => new() {
        ("name", SlotType.String),
    };
}
