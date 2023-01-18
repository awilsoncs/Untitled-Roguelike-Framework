namespace URF.Common.Effects {
  using System;
  using URF.Common;
  using URF.Common.GameEvents;
  using URF.Server.GameState;

  public abstract class BaseEffect : IEffect {

    public event EventHandler<IGameEvent> GameEvent;

    // Emit a game event from the server.
    protected void OnGameEvent(IGameEvent e) {
      GameEvent?.Invoke(this, e);
    }

    protected IGameState GameState {
      get;
      private set;
    }

    public void Apply(IEventForwarder forwarder, IGameState gameState) {
      this.GameEvent += forwarder.ForwardEvent;
      this.GameState = gameState;
      this.OnApply();
      this.GameEvent -= forwarder.ForwardEvent;
    }

    protected abstract void OnApply();
  }
}
