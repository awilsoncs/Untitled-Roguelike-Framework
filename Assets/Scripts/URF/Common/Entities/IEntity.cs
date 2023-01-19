namespace URF.Common.Entities {
  using System.Collections.Generic;
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

    /// <summary>
    /// True if this entity can attack and be attacked. False otherwise.
    /// </summary>
    bool CanFight {
      get; set;
    }

    /// <summary>
    /// This entity's maximum health.
    /// </summary>
    int MaxHealth {
      get; set;
    }

    /// <summary>
    /// This entity's current health.
    /// </summary>
    int CurrentHealth {
      get; set;
    }

    /// <summary>
    /// The amount of damage that this entity deals when attacking.
    /// </summary>
    int Damage {
      get; set;
    }

    /// <summary>
    /// A list of entity IDs for entities contained within this object.
    /// </summary>
    List<int> Inventory {
      get;
    }

    TComponentType GetComponent<TComponentType>() where TComponentType : BaseComponent;

    void AddComponent(BaseComponent component);
  }
}
