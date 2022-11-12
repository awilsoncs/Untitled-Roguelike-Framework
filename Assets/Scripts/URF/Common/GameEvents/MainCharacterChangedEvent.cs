using URF.Common.Entities;

namespace URF.Common.GameEvents {
    /// <summary>
    /// Notify listeners that the main character has changed.
    /// </summary>
    public class MainCharacterChangedEvent : GameEvent {
        /// <summary>
        /// The new main character
        /// </summary>
        public IEntity Entity { get; }
        // todo add a reference to the old main character here
        public override GameEventType EventType => GameEventType.MainCharacterChanged;

        public MainCharacterChangedEvent (IEntity entity) {
            Entity = entity;
        }
    }
}
