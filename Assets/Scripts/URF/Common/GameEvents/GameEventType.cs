namespace URF.Common.GameEvents {
  public enum GameEventType {

    EntityAttacked,

    EntityMoved,

    EntityCreated,

    EntityKilled,

    EntityVisibilityChanged,

    MainCharacterChanged,

    GameError,
    
    Configure,

    // Commands below this line

    AttackCommand,

    MoveCommand,

    DebugCommand,

    Save,
    
    Load,

    SpentTurn,
    
    Start

  }
}
