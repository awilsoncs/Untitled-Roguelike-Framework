public interface IGameState : IPersistableObject, ICommandable, IBuildable {
    void Kill(IEntity entity);
    void Log(string message);
}