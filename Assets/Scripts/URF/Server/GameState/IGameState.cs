using System.Collections.ObjectModel;
using URF.Common;
using URF.Common.Entities;

namespace URF.Server.GameState {
  public interface IGameState {

    void Delete(IEntity entity);

    (int, int) GetMapSize();

    IEntity GetEntityById(int id);

    bool IsTraversable(Position position);

    bool IsTransparent(Position position);

    void MoveEntity(int id, Position position);

    ReadOnlyCollection<IEntity> GetEntities();

    void CreateEntityAtPosition(IEntity entity, Position position);

    int MapWidth { get; }

    int MapHeight { get; }

    Cell GetCell(Position p);

  }
}
