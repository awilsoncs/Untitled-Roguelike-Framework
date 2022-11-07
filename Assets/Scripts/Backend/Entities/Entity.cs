using System.Collections.Generic;

/// <summary>
/// Represent the logical component of a game piece. Should remain agnostic to the game engine.
/// </summary>
public class Entity : IPersistableObject, IEntity {
    public int ID { get; set; }
    public bool BlocksSight { get; set; }
    public bool IsVisible { get; set; }
    public IGameState GameState {get; set;}

    private readonly Dictionary<string, int> intSlots;
    private readonly Dictionary<string, string> stringSlots;
    private readonly Dictionary<string, bool> boolSlots;

    public Entity() {
        intSlots = new();
        stringSlots = new();
        boolSlots = new();
    }

    public void SetSlot(string slotName, int value) {
        intSlots[slotName] = value;
    }

    public void SetSlot(string slotName, string value) {
        stringSlots[slotName] = value;
    }

    public void SetSlot(string slotName, bool value) {
        boolSlots[slotName] = value;
    }

    public int GetIntSlot(string slotName) {
        return intSlots[slotName];
    }

    public string GetStringSlot(string slotName) {
        return stringSlots[slotName];
    }

    public bool GetBoolSlot(string slotName) {
        return boolSlots[slotName];
    }

    public void Save(GameDataWriter writer) {
        writer.Write(BlocksSight);
        writer.Write(IsVisible);
        // general slot writing
        writer.Write(intSlots.Count);
        foreach (var key in intSlots.Keys) {
            writer.Write(key);
            writer.Write(GetIntSlot(key));
        }
        writer.Write(stringSlots.Count);
        foreach (var key in stringSlots.Keys) {
            writer.Write(key);
            writer.Write(GetStringSlot(key));
        }
        writer.Write(boolSlots.Count);
        foreach (var key in boolSlots.Keys) {
            writer.Write(key);
            writer.Write(GetBoolSlot(key));
        }
    }

    public void Load(GameDataReader reader) {
        BlocksSight = reader.ReadBool();
        IsVisible = reader.ReadBool();
        // general slot reading
        var intSlotCount = reader.ReadInt();
        for (int i = 0; i < intSlotCount; i++) {
            SetSlot(reader.ReadString(), reader.ReadInt());
        }
        var stringSlotCount = reader.ReadInt();
        for (int i = 0; i < stringSlotCount; i++) {
            SetSlot(reader.ReadString(), reader.ReadString());
        }
        var boolSlotCount = reader.ReadInt();
        for (int i = 0; i < boolSlotCount; i++) {
            SetSlot(reader.ReadString(), reader.ReadBool());
        }
    }

    public void Recycle(IEntityFactory entityFactory) {
        entityFactory.Reclaim(this);
    }

    public override string ToString()
    {
        var name = GetStringSlot("name");
        var X = GetIntSlot("X");
        var Y = GetIntSlot("Y");
        return $"{name}::{ID}::({X}, {Y})";
    }

}
