namespace URF.Server.GameState {
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.RulesSystems;

  public class GameState : BaseGameEventChannel, IGameState {

    private readonly Cell[,] map;

    public Position MapSize {
      get;
    }

    private readonly List<IEntity> entities;

    private readonly Dictionary<int, IEntity> entitiesById;

    public GameState(int mapWidth, int mapHeight) {
      this.MapSize = (mapWidth, mapHeight);

      this.entities = new List<IEntity>();
      this.entitiesById = new Dictionary<int, IEntity>();

      this.map = new Cell[this.MapSize.X, this.MapSize.Y];
      for (int i = 0; i < this.MapSize.X; i++) {
        for (int j = 0; j < this.MapSize.Y; j++) {
          this.map[i, j] = new Cell();
        }
      }
    }

    public void CreateEntityAtPosition(IEntity entity, Position position) {
      this.entities.Add(entity);
      this.entitiesById.Add(entity.ID, entity);
      this.PlaceEntity(entity.ID, position);
      this.OnGameEvent(new EntityCreated(entity, position));
    }

    public void Delete(IEntity entity) {
      int index = this.entities.FindIndex(x => x == entity);
      int lastIndex = this.entities.Count - 1;
      if (index < lastIndex) {
        this.entities[index] = this.entities[lastIndex];
      }

      this.entities.RemoveAt(lastIndex);
      _ = this.entitiesById.Remove(entity.ID);
      Position pos = entity.GetComponent<Movement>().EntityPosition;
      Cell possibleLocation = this.GetCell(pos);
      if (possibleLocation.Contents.Contains(entity)) {
        possibleLocation.RemoveEntity(entity);
      }
      this.OnGameEvent(new EntityDeleted(entity));
    }

    public Cell GetCell(Position p) {
      if (!this.IsInBounds(p)) {
        throw new ArgumentException($"Cannot get out-of bounds cell: {p} not in {this.MapSize}");
      }
      (int x, int y) = p;
      if (this.map[x, y] == null) {
        throw new ArgumentException($"Cell at {p} has not been assigned.");
      }
      return this.map[x, y];
    }

    private bool IsLegalMove(Position position) {
      return this.IsInBounds(position) && this.GetCell(position).IsTraversable;
    }

    private bool IsInBounds(Position p) {
      return 0 <= p.X && p.X < this.MapSize.X && 0 <= p.Y && p.Y < this.MapSize.Y;
    }

    private void PlaceEntity(int id, Position p) {
      IEntity entity = this.entitiesById[id];
      Cell destination = this.GetCell(p);
      entity.GetComponent<Movement>().EntityPosition = p;
      destination.PutContents(entity);
    }

    public void MoveEntity(int id, Position position) {
      if (!this.IsLegalMove(position)) {
        return;
      }

      IEntity entity = this.entitiesById[id];
      Movement movement = entity.GetComponent<Movement>();
      Position oldPos = movement.EntityPosition;
      Cell origin = this.GetCell(oldPos);

      origin.RemoveEntity(entity);
      this.PlaceEntity(id, position);
    }

    public IEntity GetEntityById(int id) {
      return this.entitiesById[id];
    }

    public ReadOnlyCollection<IEntity> GetEntities() {
      return this.entities.ToList().AsReadOnly();
    }

  }
}
