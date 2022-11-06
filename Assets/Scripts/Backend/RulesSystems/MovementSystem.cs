using System.Collections.Generic;
using URFCommon;

public class MovementSystem : IRulesSystem
{
    public void GameUpdate(IGameState gameState) {}
    public List<(GameEventType, EventHandler)> EventHandlers => new();
    public List<(string, SlotType)> Slots => new () {
        ("canMove", SlotType.Boolean),
        ("blocksMove", SlotType.Boolean),
        ("X", SlotType.Integer),
        ("Y", SlotType.Integer)
    };

    [EventHandler(GameEventType.MoveCommand)]
    public void HandleMoveCommand(IGameState gs, IGameEvent cm) {
        MoveCommand mcm = (MoveCommand)cm;
        int entityId = mcm.EntityId;
        int mx = mcm.Direction.Item1;
        int my = mcm.Direction.Item2;

        var entity = gs.GetEntityById(entityId);
        var x = entity.GetIntSlot("X") + mx;
        var y = entity.GetIntSlot("Y") + my;

        gs.MoveEntity(entityId, x, y);
        if (entityId == gs.GetMainCharacter().ID) {
            // todo eventually move this to a turn control system
            gs.GameUpdate();
        }
        // todo should probably be an EntityMoved event
        gs.RecalculateFOV();
    }
}
