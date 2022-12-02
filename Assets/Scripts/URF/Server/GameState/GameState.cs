namespace URF.Server.GameState {
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.RulesSystems;

  public class GameState : BaseGameEventChannel, IGameState {

    public int MapWidth { get; }

    public int MapHeight { get; }

    private readonly Cell[][] _map;

    private readonly List<IEntity> _entities;

    private readonly Dictionary<int, IEntity> _entitiesById;

    public GameState(int mapWidth, int mapHeight) {
      MapWidth = mapWidth;
      MapHeight = mapHeight;

      _entities = new List<IEntity>();
      _entitiesById = new Dictionary<int, IEntity>();

      _map = new Cell[MapWidth][];
      for(int i = 0; i < MapWidth; i++) {
        _map[i] = new Cell[MapHeight];
        for(int j = 0; j < MapHeight; j++) { _map[i][j] = new Cell(); }
      }
    }

    public void Delete(IEntity entity) {
      int index = this._entities.FindIndex(x => x == entity);
      int lastIndex = this._entities.Count - 1;
      if (index < lastIndex) {
        this._entities[index] = this._entities[lastIndex];
      }

      this._entities.RemoveAt(lastIndex);
      _ = this._entitiesById.Remove(entity.ID);
      Position pos = entity.GetComponent<Movement>().EntityPosition;
      Cell possibleLocation = this.GetCell(pos);
      if (possibleLocation.Contents.Contains(entity)) {
        possibleLocation.RemoveEntity(entity);
      }
      this.OnGameEvent(new EntityDeleted(entity));
    }

    public Cell GetCell(Position p) {
      (int x, int y) = p;
      return _map[x][y];
    }

    private bool IsLegalMove(Position position) {
      return IsInBounds(position) && GetCell(position).IsPassable;
    }

    private bool IsInBounds(Position p) {
      return (0 <= p.X && p.X < MapWidth && 0 <= p.Y && p.Y < MapHeight);
    }

    private void PlaceEntity(int id, Position p) {
      IEntity entity = _entitiesById[id];
      Cell destination = GetCell(p);
      entity.GetComponent<Movement>().EntityPosition = p;
      destination.PutContents(entity);
    }

    public void MoveEntity(int id, Position position) {
      if(!IsLegalMove(position)) { return; }

      IEntity entity = _entitiesById[id];
      Movement movement = entity.GetComponent<Movement>();
      Position oldPos = movement.EntityPosition;
      Cell origin = GetCell(oldPos);

      origin.RemoveEntity(entity);
      PlaceEntity(id, position);
    }

    public IEntity GetEntityById(int id) {
      return _entitiesById[id];
    }

    public ReadOnlyCollection<IEntity> GetEntities() {
      return _entities.ToList().AsReadOnly();
    }

    public (int, int) GetMapSize() {
      return (MapWidth, MapHeight);
    }

    public bool IsTraversable(Position position) {
      return GetCell(position).IsPassable;
    }

    public bool IsTransparent(Position position) {
      return GetCell(position).IsTransparent;
    }

    public void CreateEntityAtPosition(IEntity entity, Position position) {
      _entities.Add(entity);
      _entitiesById.Add(entity.ID, entity);
      PlaceEntity(entity.ID, position);
    }

  }
}
