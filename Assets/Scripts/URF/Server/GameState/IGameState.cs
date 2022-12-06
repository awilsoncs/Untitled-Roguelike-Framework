namespace URF.Server.GameState {
  using System.Collections.ObjectModel;
  using URF.Common;
  using URF.Common.Entities;

  public interface IGameState {

    void Delete(IEntity entity);

    (int, int) GetMapSize();

    IEntity GetEntityById(int id);

    void MoveEntity(int id, Position position);

    ReadOnlyCollection<IEntity> GetEntities();

    void CreateEntityAtPosition(IEntity entity, Position position);

    int MapWidth {
      get;
    }

    int MapHeight {
      get;
    }

    Cell GetCell(Position p);

  }
}
