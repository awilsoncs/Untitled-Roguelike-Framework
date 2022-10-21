using System;
using System.Collections.Generic;

/// <summary>
/// Define a set of GameState update commands provided as callbacks to the GameState.
/// </summary>
public interface IGameState : IPersistableObject {
    /// <summary>
    /// Define a user action to move left.
    /// </summary>
    void MoveLeft();
    /// <summary>
    /// Define a user action to move right.
    /// </summary>
    void MoveRight();
        /// <summary>
    /// Define a user action to move up.
    /// </summary>
    void MoveUp();
        /// <summary>
    /// Define a user action to move down.
    /// </summary>
    void MoveDown();
    /// <summary>
    /// Define a user action to move spawn a crab.
    /// </summary>
    void SpawnCrab();
    /// <summary>
    /// Get a list of all entities.
    /// </summary>
    IEnumerable<IEntity> GetEntities();
    /// <summary>
    /// Create an Entity with the specified blueprint at coordinates (x, y).
    /// </summary>
    IEntity CreateEntityAtLocation(String blueprintName, int x, int y);
    int MapWidth {get; set;}
    int MapHeight {get; set;}
}