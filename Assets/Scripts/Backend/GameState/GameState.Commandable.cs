using URFCommon;

/// <summary>
/// Define commands available to the client.
/// </summary>
// todo need to wait until the player turn to process game commands
public partial class GameState : IGameState {
    // todo move these into a pluggable handler system
    public void PushCommand(IGameCommand cm) {
        switch (cm.CommandType) {
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
                foreach(var handler in commandHandlers[cm.CommandType]) {
                    handler(this, cm);
                }
                break;
        }
    }

    private void HandleAttackCommand(AttackCommand cm) {
        if (!entitiesById.ContainsKey(cm.Attacker)) {
            PostError($"Attacking entity {cm.Attacker} does not exist.");
            return;
        }

        if (!entitiesById.ContainsKey(cm.Defender)) {
            PostError($"Defender entity {cm.Defender} does not exist.");
            return;
        }

        IEntity attacker = entitiesById[cm.Attacker];
        FighterPart fighter = attacker.GetPart<FighterPart>();
        if (fighter == null) {
            PostError($"{attacker} does not have a fighter component registered. Check the Entity definition.");
            return;
        }

        IEntity defender = entitiesById[cm.Defender];
        // todo need to push this sort of thing into an internal event system
        FighterPart defenderFighter = defender.GetPart<FighterPart>();
        if (defenderFighter == null) {
            PostError($"Illegal attack attempted...(defender {defender})");
            return;
        }

        fighter.Attack(defenderFighter);
        // todo move this event emission into the fighter component
        if (cm.Attacker == mainCharacter.ID) {
            GameUpdate();
        }
    }

    private void HandleStartGameCommand(StartGameCommand cm) {
        if (RNG == null) {
            PostError("Cannot begin game without RNG.");
            return;
        }
        if (FieldOfView == null) {
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