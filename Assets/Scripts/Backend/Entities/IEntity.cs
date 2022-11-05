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
    /// The entity's appearance descriptor.
    /// </summary>
    /// <value>A string representing the entity's appearance.</value>
    String Appearance {get; set;}

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

    void SetSlot(string slotName, int value);
    void SetSlot(string slotName, string value);
    void SetSlot(string slotName, bool value);
    int GetIntSlot(string slotName);
    string GetStringSlot(string slotName);
    bool GetBoolSlot(string slotName);
}