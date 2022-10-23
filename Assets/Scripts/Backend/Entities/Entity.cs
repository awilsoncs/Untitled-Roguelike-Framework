using System.Collections.Generic;

/// <summary>
/// Represent the logical component of a game piece. Should remain agnostic to the game engine.
/// </summary>
public class Entity : IPersistableObject, IEntity {
    public int ID { get; set; }
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Appearance {get; set;}
    public bool BlocksMove { get; set; }
    public bool BlocksSight { get; set; }
    public bool IsVisible { get; set; }
    private List<IEntityPart> parts; 

    public Entity() {
        parts = new List<IEntityPart>();
    }

    public virtual void GameUpdate(IGameState gameState) {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].GameUpdate(gameState);
        }
    }

    public void AddPart(IEntityPart part) {
        parts.Add(part);
        part.Entity = this;
    }

    public void RemovePart(IEntityPart part) {
        // todo improve this to use the end swap strategy
        // todo improve this to be reclaimed by the factory
        parts.Remove(part);
        part.Entity = null;
    }

    public void Save(GameDataWriter writer) {
        writer.Write(Name);
        writer.Write(Appearance);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(BlocksMove);
        writer.Write(BlocksSight);
        writer.Write(parts.Count);
        for (int i = 0; i < parts.Count; i++) {
            writer.Write(parts[i].Id);
            writer.Write((int)parts[i].PartType);
            parts[i].Save(writer);
        }
    }

    public void Load(GameDataReader reader) {
        Name = reader.ReadString();
        Appearance = reader.ReadString();
        X = reader.ReadInt();
        Y = reader.ReadInt();
        BlocksMove = reader.ReadBool();
        BlocksSight = reader.ReadBool();
        var partCount = reader.ReadInt();
        if (partCount > 0) {
            // todo abstract out these debug calls
            //gameClient.LogMessage($">> Entity {Name}::{ID} loading {partCount} parts...");
            for (int i = 0; i < partCount; i++) {
                var partId = reader.ReadInt();
                IEntityPart part = 
                        ((EntityPartType)reader.ReadInt()).GetInstance();
                parts.Add(part);
                part.Entity = this;
                part.Load(reader);
            }
            //Debug.Log($">> Entity {Name}::{ID} done loading parts...");
        } else {
            //Debug.Log($">> Entity {Name}::{ID}  has no parts to load...");
        }        
    }

    public void Recycle(IEntityFactory entityFactory) {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].Recycle();
        }
        parts.Clear();
        entityFactory.Reclaim(this);
    }

}
