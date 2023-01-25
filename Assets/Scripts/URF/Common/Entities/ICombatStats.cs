namespace URF.Common.Entities {
  public interface ICombatStats {

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

  }
}
