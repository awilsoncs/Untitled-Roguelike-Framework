using URF.Common.Persistence;

namespace URF.Common.Entities {
  /// <summary>
  /// High level representation of logical game objects.
  /// </summary>
  public interface IEntity : IPersistableObject {

    /// <summary>
    /// The entity's unique identifier in the game world.
    /// </summary>
    int ID { get; }

    /// <summary>
    /// Whether the entity should block field of view.
    /// </summary>
    bool BlocksSight { get; }

    /// <summary>
    /// Whether the entity is currently visible to the player.
    /// </summary>
    bool IsVisible { get; set; }
    
    T GetComponent<T>() where T : BaseComponent;

  }
}
