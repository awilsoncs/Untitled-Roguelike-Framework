namespace URF.Server.GameState {
  using System.Collections.ObjectModel;
  using URF.Common;
  using URF.Common.Entities;

  public interface IGameState {
    Position MapSize {
      get;
    }

    void Delete(IEntity entity);

    IEntity GetEntityById(int id);

    void MoveEntity(int id, Position position);

    ReadOnlyCollection<IEntity> GetEntities();

    void CreateEntityAtPosition(IEntity entity, Position position);

    Cell GetCell(Position p);

  }
}
