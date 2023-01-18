namespace URF.Common.GameEvents {
  using System;
  using URF.Common;

  public abstract class BaseGameEventChannel : IGameEventChannel {

    public event EventHandler<IGameEvent> GameEvent;

    // Emit a game event from the server.
    protected void OnGameEvent(IGameEvent e) {
      GameEvent?.Invoke(this, e);
    }

    public void HandleEvent(object _, IGameEvent ev) {
      ev.Visit(this);
      this.HandleAll(ev);
    }

    public void Connect(IGameEventChannel eventChannel) {
      eventChannel.GameEvent += this.HandleEvent;
      this.GameEvent += eventChannel.HandleEvent;
    }

    public void Listen(IGameEventChannel eventChannel) {
      eventChannel.GameEvent += this.HandleEvent;
    }

    public virtual void HandleEntityAttacked(EntityAttacked entityAttacked) {
      // no-op
    }

    public virtual void HandleAttackAction(AttackAction attackAction) {
      // default no-op
    }

    public virtual void HandleConfigure(ConfigureAction configureEvent) {
      // default no-op
    }

    public virtual void HandleDebug(DebugAction debugEvent) {
      // default no-op
    }

    public virtual void HandleEntityLocationChanged(EntityLocationChanged entityLocationChanged) {
      // default no-op
    }

    public virtual void HandleEntityVisibilityChanged(EntityVisibilityChanged visibilityChanged) {
      // default no-op
    }

    public virtual void HandleGameConfigured(GameConfigured gameConfigured) {
      // default no-op
    }

    public virtual void HandleGameErrored(GameErrored gameErrored) {
      // default no-op
    }

    public virtual void HandleGameStarted(GameStarted gameStarted) {
      // default no-op
    }

    public virtual void HandleMainCharacterChanged(MainCharacterChanged mainCharacterChanged) {
      // default no-op
    }

    public virtual void HandleMoveAction(MoveAction moveAction) {
      // default no-op
    }

    public virtual void HandleTurnSpent(TurnSpent turnSpent) {
      // default no-op
    }

    public virtual void HandleAll(IGameEvent gameEvent) {
      // default no-op
    }

    public void Ignore(IGameEvent gameEvent) {
      // default no-op
    }

    public virtual void HandleInventoryEvent(InventoryEvent inventoryEvent) {
      // default no-op
    }

    public virtual void HandlePersistenceEvent(PersistenceEvent persistenceEvent) {
      // default no-op
    }

    public virtual void HandleEffectEvent(EffectEvent effectEvent) {
      // default no-op
    }

    public virtual void HandleEntityEvent(EntityEvent effectEvent) {
      // default no-op
    }
  }
}
