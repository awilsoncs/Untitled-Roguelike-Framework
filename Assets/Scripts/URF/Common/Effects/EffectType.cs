namespace URF.Common.Effects {
  /// <summary>
  /// Represents the primitive concept of a type of effect- for instance, dealing damage or
  /// restoring health. The definitions of the effect types are managed in `EffectSystem.cs` by
  /// default, although this may not be the case if the ruleset has been customized.
  /// </summary>
  public enum EffectType {
    // A generic damage effect
    DamageHealth,
    // An effect that restores health to the affected
    RestoreHealth,
    ConsumeSource
  }
}
