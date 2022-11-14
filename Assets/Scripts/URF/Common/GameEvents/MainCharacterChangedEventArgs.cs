using System;
using URF.Common.Entities;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Notify listeners that the main character has changed.
  /// </summary>
  public class MainCharacterChangedEventArgs : EventArgs, IGameEventArgs {

    /// <summary>
    /// The new main character
    /// </summary>
    public IEntity Entity { get; }

    // todo add a reference to the old main character here
    public GameEventType EventType => GameEventType.MainCharacterChanged;

    public MainCharacterChangedEventArgs(IEntity entity) {
      Entity = entity;
    }

  }
}
