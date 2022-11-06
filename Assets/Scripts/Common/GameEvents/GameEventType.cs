namespace URFCommon {
    public enum GameEventType {
        EntityAttacked,
        EntityMoved,
        EntityCreated,
        EntityKilled,
        EntityVisibilityChanged,
        MainCharacterChanged,
        GameError,
        GameStarted,
        // Commands below this line
        StartGameCommand,
        AttackCommand,
        MoveCommand,
        DebugCommand
    }
}
