using System;
using System.Collections.Generic;
using URFCommon;

public class MovementSystem : BaseRulesSystem
{
    public override List<Type> Components => new () {
        // todo could create an annotation to register these
        typeof(Movement)
    };

    [EventHandler(GameEventType.MoveCommand)]
    public void HandleMoveCommand(IGameState gs, IGameEvent cm) {
        MoveCommand mcm = (MoveCommand)cm;
        int entityId = mcm.EntityId;

        var entity = gs.GetEntityById(entityId);
        var newPos = entity.GetComponent<Movement>().Position + mcm.Direction;

        gs.MoveEntity(entityId, newPos);
        if (entityId == gs.GetMainCharacter().ID) {
            // todo eventually move this to a turn control system
            gs.GameUpdate();
        }
        // todo should probably be an EntityMoved event
        gs.RecalculateFOV();
    }
}

public class Movement : BaseComponent
{
    public bool CanMove;
    public bool BlocksMove;
    public Position Position;
    public override void Load(GameDataReader reader)
    {
        CanMove = reader.ReadBool();
        BlocksMove = reader.ReadBool();
        Position = (reader.ReadInt(), reader.ReadInt());
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(CanMove);
        writer.Write(BlocksMove);
        writer.Write(Position.X);
        writer.Write(Position.Y);
    }
}