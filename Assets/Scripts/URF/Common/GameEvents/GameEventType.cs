namespace URF.Common.GameEvents {
  public enum GameEventType {

    EntityAttacked,

    EntityMoved,

    EntityCreated,

    EntityKilled,

    EntityVisibilityChanged,

    MainCharacterChanged,

    GameError,
    
    StartGame,

    // Commands below this line

    AttackCommand,

    MoveCommand,

    DebugCommand,

    Save,
    
    Load

  }
}
