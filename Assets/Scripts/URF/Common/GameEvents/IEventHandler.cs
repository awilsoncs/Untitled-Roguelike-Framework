namespace URF.Common.GameEvents {
  public interface IEventHandler {
    void HandleAll(IGameEvent gameEvent);
    void Ignore(IGameEvent gameEvent);
    void HandleEntityCreated(EntityCreated entityCreated);
    void HandleEntityDeleted(EntityDeleted entityKilled);

    void HandleAttackAction(AttackAction attackAction);
    void HandleConfigure(ConfigureAction configureEvent);
    void HandleDebug(DebugAction debugEvent);
    void HandleEntityAttacked(EntityAttacked entityAttack);
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
    void HandleGetAction(GetAction getAction);

  }
}
