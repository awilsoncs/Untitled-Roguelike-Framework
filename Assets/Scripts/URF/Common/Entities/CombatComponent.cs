namespace URF.Common.Entities {
  using URF.Common.Persistence;

  /// <summary>
  /// Represent combat information about an entity.
  /// </summary>
  public class CombatComponent : BaseComponent {

    /// <summary>
    /// True if this entity can attack and be attacked. False otherwise.
    /// </summary>
    public bool CanFight {
      get; set;
    }

    /// <summary>
    /// This entity's maximum health.
    /// </summary>
    public int MaxHealth {
      get; set;
    }

    /// <summary>
    /// This entity's current health.
    /// </summary>
    public int CurrentHealth {
      get; set;
    }

    /// <summary>
    /// The amount of damage that this entity deals when attacking.
    /// </summary>
    public int Damage {
      get; set;
    }

    /// <inheritdoc>
    public override void Load(IGameDataReader reader) {
      this.CanFight = reader.ReadBool();
      this.MaxHealth = reader.ReadInt();
      this.CurrentHealth = reader.ReadInt();
      this.Damage = reader.ReadInt();
    }

    /// <inheritdoc>
    public override void Save(IGameDataWriter writer) {
      writer.Write(this.CanFight);
      writer.Write(this.MaxHealth);
      writer.Write(this.CurrentHealth);
      writer.Write(this.Damage);
    }

  }
}
