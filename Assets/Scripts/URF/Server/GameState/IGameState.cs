using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;

namespace URF.Server.GameState {
  public interface IGameState {

    void Kill(IEntity entity);

    (int, int) GetMapSize();

    IEntity GetEntityById(int id);

    bool IsTraversable(Position position);

    void MoveEntity(int id, Position position);

    List<IEntity> GetEntities();
    
    void CreateEntityAtPosition(IEntity entity, Position position);

    int MapWidth { get; }

    int MapHeight { get; }

    Cell GetCell(Position p);

    void BeginUpdate();

    void FinishUpdate();

    void SetMainCharacter(int id);
    
  }
}
