namespace URF.Common.Effects {
  using URF.Common.Persistence;

  /// <summary>
  /// EffectSpecs are used to define the prototype of an effect, without regard for the agent,
  /// source, or targets. EffectSpecs are attached to entities via the Useable component, and
  /// should be persisted along with the entity.
  /// </summary>
  public interface IEffectSpec : IPersistableObject {

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
