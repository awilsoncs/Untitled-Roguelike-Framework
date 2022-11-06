namespace URFCommon {
    /// <summary>
    /// An event from the game state to the client.
    /// </summary>
    public interface IGameEvent {
        /// <summary>
        /// Provide the event type for parsing on the client side.
        /// </summary>
        /// <value>The corresponding event type for this event.</value>
        GameEventType EventType {get;}

        /// <summary>
        /// Designate whether this event is an imperative command. Commands are
        /// not sent from the game state to the client.
        /// </summary>
        bool IsCommand {get;}
    }
}
