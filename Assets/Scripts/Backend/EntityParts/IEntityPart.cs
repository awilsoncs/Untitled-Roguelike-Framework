using UnityEngine;

public interface IEntityPart : IPersistableObject
 {
    /// <summary>
    /// The unique identifier of the part within game space.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// A descriptor of the part type (for serialization).
    /// </summary>
    EntityPartType PartType {get;}

    /// <summary>
    /// The IEntity to which this is attached.
    /// </summary>
    IEntity Entity {get; set;}

    /// <summary>
    /// The IGameState in which this lives.
    /// </summary>
    IGameState GameState {get; set;}


    /// <summary>
    /// Perform game loop updates.
    /// </summary>
    void GameUpdate() {}

    /// <summary>
    /// Reclaim this IEntityPart
    /// </summary>
    void Recycle();

// unity reference inside the no-unity zone, but required for editor-specific
// behavior
#if UNITY_EDITOR
    // marked by pools to indicate this object has been reclaimed
    // see OnEnable.
    bool IsReclaimed { get; set; }

    void OnEnable () {}
#endif
}