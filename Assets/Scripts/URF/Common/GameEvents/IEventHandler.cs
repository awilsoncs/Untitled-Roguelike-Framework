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
    void HandleEntityLocationChanged(EntityLocationChanged entityLocationChanged);
    void HandleEntityVisibilityChanged(EntityVisibilityChanged visibilityChanged);
    void HandleGameConfigured(GameConfigured gameConfigured);
    void HandleGameErrored(GameErrored gameErrored);
    void HandleGameStarted(GameStarted gameStarted);
    void HandleMainCharacterChanged(MainCharacterChanged mainCharacterChanged);
    void HandleMoveAction(MoveAction moveAction);
    void HandleTurnSpent(TurnSpent turnSpent);
    void HandleInventoryEvent(InventoryEvent inventoryEvent);
    void HandlePersistenceEvent(PersistenceEvent persistenceEvent);
  }
}
