using System;

/// <summary>
/// High level container for logical game objects.
/// </summary>
public interface IEntity : IPersistableObject {
    /// <summary>
    /// The entity's unique identifier in the game world.
    /// </summary>
    int ID { get; set; }
    
    /// <summary>
    /// The entity's (non-unique) name in the game world.
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// The entity's horizontal coordinate in game space.
    /// </summary>
    int X { get; set; }

    /// <summary>
    /// The entity's vertical coordinate in game space.
    /// </summary>
    int Y { get; set; }

    /// <summary>
    /// The entity's appearance descriptor.
    /// </summary>
    /// <value>A string representing the entity's appearance.</value>
    String Appearance {get; set;}

    /// <summary>
    /// Whether the entity should block movement.
    /// </summary>
    bool BlocksMove { get; set; }

    /// <summary>
    /// Whether the entity should block field of view.
    /// </summary>
    bool BlocksSight { get; set; }

    /// <summary>
    /// Whether the entity is currently visible to the player.
    /// </summary>
    bool IsVisible { get; set; }

    /// <summary>
    /// Perform game loop updates.
    /// </summary>
    /// <param name="gameState">A reference to the game state.</param>
    void GameUpdate(IGameState gameState);

    /// <summary>
    /// Add an IEntityPart to the Entity.
    /// </summary>
    /// <param name="part">The IEntityPart to add.</param>
    void AddPart(IEntityPart part);

    /// <summary>
    /// Remove an IEntityPart from the Entity.
    /// </summary>
    /// <param name="part">The IEntityPart to remove.</param>
    void RemovePart(IEntityPart part);

    /// <summary>
    /// Return a part of the given type.
    /// </summary>
    /// <typeparam name="T">An IEntityPArt.</typeparam>
    /// <returns></returns>
    T GetPart<T>() where T : IEntityPart;

    /// <summary>
    /// Recycle this entity at the given factory.
    /// </summary>
    /// <param name="entityFactory">A reference to a factory that can dispose 
    /// of this entity properly.</param>
    void Recycle(IEntityFactory entityFactory);

    
}