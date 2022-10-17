using System.Collections.Generic;
using UnityEngine;

public class Entity : PersistableObject{
    public int EntityType {get; set;}
    public int ID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int SpriteIndex {get; set;}

    private List<EntityPart> parts; 
    private SpriteRenderer spriteRenderer;

    public Entity() {
        parts = new List<EntityPart>();
    }

    private void OnEnable() {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public virtual void GameUpdate() {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].GameUpdate();
        }
    }

    public void Move(int dx, int dy) {
        TeleportTo(X+dx, Y+dy);
    }

    public void TeleportTo(int x, int y) {
        X = x;
        Y = y;
        BoardController.Instance.SetPawnPosition(ID, X, Y);
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

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(gameObject.name);
        writer.Write(SpriteIndex);
        writer.Write(X);
        writer.Write(Y);
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
        var spriteIndex = reader.ReadInt();

        SpriteIndex = spriteIndex;
        SetSprite(BoardController.Instance.entityFactory.GetSpriteByIndex(spriteIndex));

        X = reader.ReadInt();
        Y = reader.ReadInt();
        var partCount = reader.ReadInt();
        if (partCount > 0) {
            Debug.Log($">> Entity {gameObject.name}::{ID} loading {partCount} parts...");
            for (int i = 0; i < partCount; i++) {
                var partId = reader.ReadInt();
                EntityPart part = 
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

    public void Recycle() {
        for (int i = 0; i < parts.Count; i++) {
            parts[i].Recycle();
        }
        parts.Clear();
        BoardController.Instance.entityFactory.Reclaim(this);
    }

}
