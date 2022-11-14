using System;
using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class MovementSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        // todo could create an annotation to register these
        typeof(Movement)
      };

    [ActionHandler(GameEventType.MoveCommand)]
    public void HandleMoveCommand(IGameState gs, IActionEventArgs cm) {
      MoveActionEventArgs mcm = (MoveActionEventArgs)cm;
      int entityId = mcm.EntityId;

      IEntity entity = gs.GetEntityById(entityId);
      Position newPos = entity.GetComponent<Movement>().EntityPosition + mcm.Direction;

      gs.MoveEntity(entityId, newPos);
      OnGameEvent(new TurnSpentEventArgs(entity));
    }

  }

  public class Movement : BaseComponent {

    public bool BlocksMove { get; set; }

    public Position EntityPosition { get; set; }

    public override void Load(GameDataReader reader) {
      BlocksMove = reader.ReadBool();
      EntityPosition = (reader.ReadInt(), reader.ReadInt());
    }

    public override void Save(GameDataWriter writer) {
      writer.Write(BlocksMove);
      writer.Write(EntityPosition.X);
      writer.Write(EntityPosition.Y);
    }

  }
}
