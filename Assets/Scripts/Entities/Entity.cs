using System.Collections.Generic;
using UnityEngine;

public class Entity : PersistableObject, IEntity {
    public int ID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int SpriteIndex {get; set;}
    public bool BlocksMove { get; set; }
    public bool BlocksSight { get; set; }

    private List<IEntityPart> parts; 
    private SpriteRenderer spriteRenderer;

    public Entity() {
        parts = new List<IEntityPart>();
    }

    private void OnEnable() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public virtual void GameUpdate(IBoardController bc) {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].GameUpdate(bc);
        }
    }

    /*
    * Add a part to the entity's part list.
    */
    public void AddPart(EntityPart part) {
        parts.Add(part);
        part.Entity = this;
    }

    /*
    * Remove a part from the entity's part set.
    */
    public void RemovePart(EntityPart part) {
        // todo improve this to use the end swap strategy
        // todo improve this to be reclaimed by the factory
        parts.Remove(part);
        part.Entity = null;
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }

    public void SetSpritePosition(float x, float y) {
        gameObject.transform.position = new Vector3(x, y, 0f);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(gameObject.name);
        writer.Write(X);
        writer.Write(Y);
        writer.Write(BlocksMove);
        writer.Write(BlocksSight);
        writer.Write(parts.Count);
        for (int i = 0; i < parts.Count; i++) {
            writer.Write(parts[i].ID);
            writer.Write((int)parts[i].PartType);
            parts[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        gameObject.name = reader.ReadString();
        X = reader.ReadInt();
        Y = reader.ReadInt();
        BlocksMove = reader.ReadBool();
        BlocksSight = reader.ReadBool();
        var partCount = reader.ReadInt();
        if (partCount > 0) {
            Debug.Log($">> Entity {gameObject.name}::{ID} loading {partCount} parts...");
            for (int i = 0; i < partCount; i++) {
                var partId = reader.ReadInt();
                IEntityPart part = 
                        ((EntityPartType)reader.ReadInt()).GetInstance();
                parts.Add(part);
                part.Entity = this;
                part.Load(reader);
            }
            Debug.Log($">> Entity {gameObject.name}::{ID} done loading parts...");
        } else {
            Debug.Log($">> Entity {gameObject.name}::{ID}  has no parts to load...");
        }
        
    }

    public void Recycle(EntityFactory entityFactory) {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].Recycle();
        }
        parts.Clear();
        entityFactory.Reclaim(this);
    }

}
