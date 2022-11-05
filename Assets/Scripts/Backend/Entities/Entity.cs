using System.Collections.Generic;

/// <summary>
/// Represent the logical component of a game piece. Should remain agnostic to the game engine.
/// </summary>
public class Entity : IPersistableObject, IEntity {
    public int ID { get; set; }
    public string Appearance {get; set;}
    public bool BlocksMove { get; set; }
    public bool BlocksSight { get; set; }
    public bool IsVisible { get; set; }
    private List<IEntityPart> parts; 
    private IGameState _gameState;
    public IGameState GameState { 
        get {
            return _gameState;
        }
        set {
            // if the entity's game state ref changes, we need to update the
            // parts.
            _gameState = value;
            for (int i = 0; i < parts.Count; i++) {
                parts[i].GameState = _gameState;
            }
        }
    }

    private readonly Dictionary<string, int> intSlots;
    private readonly Dictionary<string, string> stringSlots;
    private readonly Dictionary<string, bool> boolSlots;

    public Entity() {
        parts = new();
        intSlots = new();
        stringSlots = new();
        boolSlots = new();
    }

    public virtual void GameUpdate(IGameState gameState) {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].GameUpdate();
        }
    }

    public void AddPart(IEntityPart part) {
        parts.Add(part);
        part.Entity = this;
        part.GameState = this.GameState;
    }

    public void RemovePart(IEntityPart part) {
        // todo improve this to use the end swap strategy
        // todo improve this to be reclaimed by the factory
        parts.Remove(part);
        part.Entity = null;
        part.GameState = null;
    }

    public T GetPart<T>() where T : IEntityPart {
        // Find a matching part and return it.
        // todo could make this a map to speed up the lookup.
        return (T)parts.Find((IEntityPart x) => {return x is T;});
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
        writer.Write(Appearance);
        writer.Write(BlocksSight);
        writer.Write(IsVisible);
        writer.Write(parts.Count);
        for (int i = 0; i < parts.Count; i++) {
            writer.Write((int)parts[i].PartType);
            parts[i].Save(writer);
        }
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
        Appearance = reader.ReadString();
        BlocksSight = reader.ReadBool();
        IsVisible = reader.ReadBool();
        var partCount = reader.ReadInt();
        for (int i = 0; i < partCount; i++) {
            IEntityPart part = 
                    ((EntityPartType)reader.ReadInt()).GetInstance();
            parts.Add(part);
            part.Entity = this;
            part.GameState = this.GameState;
            part.Load(reader);
        }
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
        for (int i = 0; i < parts.Count; i++) {
            parts[i].Recycle();
            parts[i].Entity = null;
            parts[i].GameState = null;
        }
        parts.Clear();
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
