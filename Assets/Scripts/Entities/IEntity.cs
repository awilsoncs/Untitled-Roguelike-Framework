using System;

public interface IEntity : IPersistableObject {
    int ID { get; set; }
    int X { get; set; }
    int Y { get; set; }
    String Appearance {get; set;}
    bool BlocksMove { get; set; }
    bool BlocksSight { get; set; }

    void GameUpdate(IGameState gameState);
    void AddPart(EntityPart part);
    void RemovePart(EntityPart part);
    void Recycle(EntityFactory entityFactory);
}