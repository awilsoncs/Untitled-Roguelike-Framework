using System;

/// <summary>
/// Provide a pluggable system for updating a client to a GameState.
/// </summary>
public interface IGameClient {
    /// <summary>
    /// Inform the client that an entity has moved.
    /// </summary>
    /// <param name="entityId">The entity's game state id.</param>
    /// <param name="x">The entity's horizontal game state position.</param>
    /// <param name="y">The entity's vertical game state position</param>
    void EntityMoved(int entityId, int x, int y);
    /// <summary>
    /// Inform the client that an entity has been created.
    /// </summary>
    /// <param name="id">The entity's game state id.</param>
    /// <param name="appearance">The entity's appearance descriptor.</param>
    /// <param name="x">The entity's horizontal game state position.</param>
    /// <param name="y">The entity's vertical game state position</param>
    void EntityCreated(int id, string appearance, int x, int y);
    /// <summary>
    /// Inform the client that an entity (object) has been killed.
    /// </summary>
    /// <param name="id">The entity's game state id.</param>
    void EntityKilled(int id);
    // void EntityAppearanceChanged(int id, string newAppearance);
    // void EntityBecameVisible(int id);
    // void EntityBecameInVisible(int id);
    /// <summary>
    /// Send a log message to the client.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void LogMessage(System.String message);
}