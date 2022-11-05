using URFCommon;

public interface IGameState : IPersistableObject, ICommandable, IBuildable {
    IFieldOfView FieldOfView {get;}
    IPathfinding Pathfinding {get;}
    void Kill(IEntity entity);
    void Log(string message);
    (int, int) GetMapSize();
    bool IsTraversable(int i, int j);
    IEntity GetMainCharacter();
    void PostEvent(IGameEvent ev);
    IEntity GetEntityById(int id);
    void GameUpdate();
    void MoveEntity(int id, int x, int y);
    void RecalculateFOV();
}