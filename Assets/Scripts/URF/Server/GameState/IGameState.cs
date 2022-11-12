using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.FieldOfView;
using URF.Server.Pathfinding;
using URF.Server.RandomGeneration;

namespace URF.Server.GameState {
  public interface IGameState : IPersistableObject, IBuildable {

    IFieldOfView FieldOfView { get; }

    IPathfinding Pathfinding { get; }

    IRandomGenerator Random { get; }

    void Kill(IEntity entity);

    void Log(string message);

    (int, int) GetMapSize();

    IEntity GetMainCharacter();

    void PostEvent(IGameEvent ev);

    void PostError(string message);

    bool EntityExists(int id);

    IEntity GetEntityById(int id);

    void GameUpdate();

    bool IsTraversable(Position position);

    void MoveEntity(int id, Position position);

    void RecalculateFOV();

    void RecalculateFOVImmediately();

    List<IEntity> GetEntities();

  }
}
