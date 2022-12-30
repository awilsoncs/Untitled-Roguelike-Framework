namespace URF.Common.GameEvents {
  using System;

  /// <summary>
  /// Notify listeners that some event related to persistence has occurred, including the player
  /// requesting requesting a save or load.
  /// </summary>
  public class PersistenceEvent : EventArgs, IGameEvent {

    /// <summary>
    /// Designate the specifics of the persistence event.
    /// </summary>
    public enum PersistenceEventSubtype {
      /// <summary>
      /// The player has requested that the game be saved.
      /// </summary>
      SaveRequested,
      /// <summary>
      /// The player has requested that the game be loaded.
      /// </summary>
      LoadRequested
    }

    /// <summary>
    /// The specific type of persistence event that has occurred.
    /// </summary>
    public PersistenceEventSubtype Subtype {
      get;
    }

    public PersistenceEvent(PersistenceEventSubtype subType) {
      this.Subtype = subType;
    }

    /// <summary>
    /// Create a SaveRequested PersistenceEvent.
    /// </summary>
    public static PersistenceEvent SaveRequested() {
      return new(PersistenceEventSubtype.SaveRequested);
    }

    /// <summary>
    /// Create a LoadRequested PersistenceEvent.
    /// </summary>
    public static PersistenceEvent LoadRequested() {
      return new(PersistenceEventSubtype.LoadRequested);
    }

    /// <inheritdoc />
    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandlePersistenceEvent(this);
    }
  }

}
