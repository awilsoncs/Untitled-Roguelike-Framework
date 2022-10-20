using UnityEngine;

public interface IEntity : IPersistableObject {
    int ID { get; set; }
    int X { get; set; }
    int Y { get; set; }
    int SpriteIndex {get; set;}
    bool BlocksMove { get; set; }
    bool BlocksSight { get; set; }

    void GameUpdate(IBoardController boardController);
    void AddPart(EntityPart part);
    void RemovePart(EntityPart part);
    void SetSprite(Sprite sprite);
    void SetSpritePosition(float x, float y);
    void Recycle(EntityFactory entityFactory);
}