using System.Collections.Generic;

/// <summary>
/// Define commands available to the client.
/// </summary>
// todo need to wait until the player turn to process game commands
public partial class GameState : IGameState {
    // todo move these into a pluggable handler system
    public void PushCommand(IGameCommand cm) {
        switch (cm.CommandType) {
            case GameCommandType.Move:
                HandleMoveCommand((MoveCommand)cm);
                return;
            case GameCommandType.Attack:
                HandleAttackCommand((AttackCommand)cm);
                return;
            case GameCommandType.StartGame:
                HandleStartGameCommand((StartGameCommand)cm);
                return;
            case GameCommandType.Debug:
                HandleDebugCommand((DebugCommand)cm);
                return;
            default:
                PostError($"Unrecognized command {cm.CommandType}");
                return;
        }
    }

    private void HandleMoveCommand(MoveCommand cm) {
        int entityId = cm.EntityId;
        int mx = cm.Direction.Item1;
        int my = cm.Direction.Item2;

        if (mainCharacter == null) {
            PostError("mainCharacter has not been set!");
            return;
        }

        var entity = entitiesById[entityId];
        var x = entity.X + mx;
        var y = entity.Y + my;
        MoveEntity(entityId, x, y);
        if (entityId == mainCharacter.ID) {
            GameUpdate();
        }
        RecalculateFOV();
    }

    private void HandleAttackCommand(AttackCommand cm) {
        if (!entitiesById.ContainsKey(cm.Defender)) {
            PostError($"Defender entity {cm.Defender} does not exist.");
            return;
        }
        FighterPart mainFighter = mainCharacter.GetPart<FighterPart>();
        if (mainFighter == null) {
            PostError($"Main character does not have a fighter component registered. Check the Entity definition.");
            return;
        }
        IEntity defender = entitiesById[cm.Defender];
        // todo need to push this sort of thing into an internal event system
        FighterPart defenderFighter = defender.GetPart<FighterPart>();
        if (defenderFighter == null) {
            PostError($"Illegal attack attempted...(defender {defender})");
            return;
        }
        mainFighter.Attack(defenderFighter);
        // todo move this event emission into the fighter component
        PostEvent(new EntityAttackedEvent(mainCharacter.ID, cm.Defender, true, 1));
        GameUpdate();
    }

    private void HandleStartGameCommand(StartGameCommand cm) {
        if (RNG == null) {
            PostError("Cannot begin game without RNG.");
            return;
        }
        if (fieldOfView == null) {
            PostError("Cannot begin game without FOV plugin.");
            return;
        }
        DungeonBuilder.Build(this, RNG);
        RecalculateFOVImmediately();
        gameClient.PostEvent(new GameStartedEvent());
    }

    private void HandleDebugCommand(DebugCommand cm) {
        switch (cm.Method) {
            case DebugCommand.DebugMethod.SpawnCrab:
                CreateEntityAtPosition(
                    "crab",
                    RNG.GetInt(1, MapWidth-2),
                    RNG.GetInt(1, MapHeight-2)
                );
                return;
            default:
                PostError($"Unknown debug method {cm.Method}");
                return;
        }
    }
}