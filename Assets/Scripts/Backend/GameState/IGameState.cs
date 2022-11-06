using System.Collections.Generic;
using URFCommon;

public interface IGameState : IPersistableObject, IBuildable {
    IFieldOfView FieldOfView {get;}
    IPathfinding Pathfinding {get;}
    IRandomGenerator RNG {get;}
    void Kill(IEntity entity);
    void Log(string message);
    (int, int) GetMapSize();
    IEntity GetMainCharacter();
    void PostEvent(IGameEvent ev);
    void PostError(string message);
    bool EntityExists(int id);
    IEntity GetEntityById(int id);
    void GameUpdate();
    bool IsTraversable(Position p);
    void MoveEntity(int id, Position p);
    void RecalculateFOV();
    void RecalculateFOVImmediately();
    List<IEntity> GetEntities();
}