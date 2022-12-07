namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Common.Persistence;

  public class MovementSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        typeof(Movement)
      };

    public override void HandleMoveAction(MoveAction ev) {
      IEntity entity = ev.Entity;
      Position newPosition = this.GameState.LocateEntityOnMap(entity) + ev.Direction;

      this.GameState.MoveEntity(entity, newPosition);
      this.OnGameEvent(new TurnSpent(entity));
    }

  }

  public class Movement : BaseComponent {

    public bool BlocksMove {
      get; set;
    }

    public override void Load(IGameDataReader reader) {
      this.BlocksMove = reader.ReadBool();
    }

    public override void Save(IGameDataWriter writer) {
      writer.Write(this.BlocksMove);
    }

  }
}
