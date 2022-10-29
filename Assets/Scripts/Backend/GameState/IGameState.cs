public interface IGameState : IPersistableObject, ICommandable, IBuildable {
    IPathfinding Pathfinding {get;}
    void Kill(IEntity entity);
    void Log(string message);
    (int, int) GetMapSize();
    bool IsTraversable(int i, int j);
    IEntity GetMainCharacter();
}