namespace URF.Common.Effects {

  /// <summary>
  /// Basic implementation of IEffect.
  /// </summary>
  public class Effect : IEffect {

    /// <inheritdoc />
    public EffectType Type {
      get;
    }

    /// <inheritdoc />
    public int Magnitude {
      get;
    }

    public Effect(EffectType type, int magnitude) {
      this.Type = type;
      this.Magnitude = magnitude;
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
