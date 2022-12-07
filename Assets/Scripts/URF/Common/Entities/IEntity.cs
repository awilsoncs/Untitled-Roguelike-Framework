namespace URF.Common.Entities {
  using URF.Common.Persistence;

  /// <summary>
  /// High level representation of logical game objects.
  /// </summary>
  public interface IEntity : IPersistableObject {

    /// <summary>
    /// The entity's unique identifier in the game world.
    /// </summary>
    int ID {
      get; set;
    }

    /// <summary>
    /// Whether the entity should block field of view.
    /// </summary>
    bool BlocksSight {
      get; set;
    }

    /// <summary>
    /// Whether the entity is currently visible to the player.
    /// </summary>
    bool IsVisible {
      get; set;
    }

    TComponentType GetComponent<TComponentType>() where TComponentType : BaseComponent;

    void AddComponent(BaseComponent component);
  }
}
