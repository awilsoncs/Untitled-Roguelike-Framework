using System;
using UnityEngine;
using URF.Common;
using URF.Common.GameEvents;

namespace URF.Server {
  public abstract class BaseGameEventChannel : MonoBehaviour, IGameEventChannel {

    public event EventHandler<IGameEventArgs> GameEvent;

    // Emit a game event from the server.
    protected virtual void OnGameEvent(IGameEventArgs e) {
      GameEvent?.Invoke(this, e);
    }

    public void Connect(IPlayerActionChannel actionChannel) {
      actionChannel.PlayerAction += HandleAction;
    }

    protected abstract void HandleAction(object sender, IActionEventArgs ev);

  }
}
