namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  /// <summary>
  /// Notify listeners that the main character has changed.
  /// </summary>
  public class MainCharacterChanged : EventArgs, IGameEvent {

    /// <summary>
    /// The new main character
    /// </summary>
    public IEntity Entity {
      get;
    }

    // todo add a reference to the old main character here
    public MainCharacterChanged(IEntity entity) {
      this.Entity = entity;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleMainCharacterChanged(this);
    }
  }
}
