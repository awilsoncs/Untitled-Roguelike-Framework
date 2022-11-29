namespace URF.Common.GameEvents {
  public interface IEventHandler {
    void HandleAll(IGameEvent gameEvent);
    void Ignore(IGameEvent gameEvent);
    void HandleAttackAction(AttackAction attackAction);
    void HandleConfigure(ConfigureAction configureEvent);
    void HandleDebug(DebugAction debugEvent);
    void HandleEntityAttacked(EntityAttacked entityAttack);
    void HandleEntityCreated(EntityCreated entityCreated);
    void HandleEntityKilled(EntityKilled entityKilled);
    void HandleEntityMoved(EntityMoved entityMoved);
    void HandleEntityVisibilityChanged(EntityVisibilityChanged visibilityChanged);
    void HandleGameConfigured(GameConfigured gameConfigured);
    void HandleGameErrored(GameErrored gameErrored);
    void HandleGameStarted(GameStarted gameStarted);
    void HandleLoad(LoadAction load);
    void HandleMainCharacterChanged(MainCharacterChanged mainCharacterChanged);
    void HandleMoveAction(MoveAction moveAction);
    void HandleSaveAction(SaveAction saveAction);
    void HandleTurnSpent(TurnSpent turnSpent);
  }
}