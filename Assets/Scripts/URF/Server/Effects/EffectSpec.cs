namespace URF.Server.Effects {
  using System;
  using URF.Common.Effects;
  using URF.Common.Persistence;

  /// <summary>
  /// This is the default EffectSpec implementation. See IEffectSpec for details.
  /// </summary>
  public class EffectSpec : IEffectSpec {

    /// <value>Property <c>Type</c> represents the EffectSpec's effect type. </value>
    public EffectType Type {
      get;
      set;
    }

    /// <value>Property <c>Magnitude</c> represents the EffectSpec's relative magnitude. </value>
    public int Magnitude {
      get;
      set;
    }

    /// <summary>
    /// Default constructor for to support loading effect data. This will be invoked by the owning
    /// structure- <c>Useable</c> in the default case.
    /// </summary>
    public EffectSpec() {
    }

    /// <summary>
    /// The general purpose constructor for use in the <c>EntityFactory</c>.
    /// </summary>
    public EffectSpec(EffectType type, int magnitude) {
      this.Type = type;
      this.Magnitude = magnitude;
    }

    /// <summary>
    /// Load the <c>EffectSpec</c>. Should be invoked on an existing <c>EffectSpec</c> by the
    /// owning object.
    /// </summary>
    public void Save(IGameDataWriter writer) {
      if (writer == null) {
        throw new ArgumentNullException(nameof(writer));
      }
      writer.Write((int)this.Type);
      writer.Write(this.Magnitude);
    }

    /// <summary>
    /// Load the <c>EffectSpec</c>. Should be invoked on a new <c>EffectSpec</c> by the owning
    /// object.
    /// </summary>
    public void Load(IGameDataReader reader) {
      if (reader == null) {
        throw new ArgumentNullException(nameof(reader));
      }
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
    public static IEffectSpec WithMagnitude(this EffectType effectType, int magnitude) {
      return new EffectSpec(effectType, magnitude);
    }
  }
}
