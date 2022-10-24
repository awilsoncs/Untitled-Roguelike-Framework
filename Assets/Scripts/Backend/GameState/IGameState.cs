public interface IGameState : IPersistableObject, ICommandable, IBuildable {
    void Kill(IEntity entity);
}