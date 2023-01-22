namespace URF.Common.Effects {

  /// <summary>
  /// A prototypical effect.
  /// </summary>
  public interface IEffect {

    /// <summary>
    /// The type of the effect.
    /// </summary>
    EffectType Type {
      get;
    }

    /// <summary>
    /// The magnitude of the effect. The nature of the magnitude depends on the effect- for instance
    /// this may be a percentage between 0 and 100, an ID in the bestiary, etc.
    /// </summary>
    int Magnitude {
      get;
    }
  }
}
