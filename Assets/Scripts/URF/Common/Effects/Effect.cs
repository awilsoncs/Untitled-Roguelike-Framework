namespace URF.Common.Effects {
  using URF.Common.Persistence;

  /// <summary>
  /// Basic implementation of IEffect.
  /// </summary>
  public class Effect : IEffect {

    /// <inheritdoc />
    public EffectType Type {
      get;
      set;
    }

    /// <inheritdoc />
    public int Magnitude {
      get;
      set;
    }

    public Effect() {
      // We need the default constructor to support serialization patterns.
    }

    public Effect(EffectType type, int magnitude) {
      this.Type = type;
      this.Magnitude = magnitude;
    }

    public void Save(IGameDataWriter writer) {
      writer.Write((int)this.Type);
      writer.Write(this.Magnitude);
    }

    public void Load(IGameDataReader reader) {
      this.Type = (EffectType)reader.ReadInt();
      this.Magnitude = reader.ReadInt();
    }
  }

  public static class EffectTypeExtensions {
    /// <summary>
    /// Provide an alternate constructor based on EffectType.
    /// </summary>
    /// <param name="effectType">The root effect type.</param>
    /// <param name="magnitude">The magnitude of the effect</param>
    /// <returns>An Effect with type and magnitude equal to those given.</returns>
    public static Effect WithMagnitude(this EffectType effectType, int magnitude) {
      return new Effect(effectType, magnitude);
    }
  }
}
