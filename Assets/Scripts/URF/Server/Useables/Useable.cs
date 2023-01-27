namespace URF.Server.Useables {
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Persistence;
  using URF.Common.Useables;
  using URF.Server.Effects;
  using URF.Server.Resolvables;

  /// <summary>
  /// A useable, e.g. an item or ability.
  /// </summary>
  public class Useable : IUseable {

    /// <value>Represent the targeting strategy for the Useable, e.g. Self, OneCreature.</value>
    public TargetScope Scope {
      get; set;
    }

    /// <value>The costs that must be paid when this useable is used.</value>
    public IEnumerable<IEffectSpec> Costs => this.costs;
    private List<IEffectSpec> costs;

    /// <value>The effects of the Useable when it resolves.</value>
    public IEnumerable<IEffectSpec> Effects => this.effects;
    private List<IEffectSpec> effects;

    public Useable() {
      // We need the default constructor to support serialization patterns.
    }

    /// <summary>
    /// Simple constructor with a single effect and no costs.
    public Useable(TargetScope scope, IEffectSpec effect) : this(
      scope,
      new List<IEffectSpec>() { effect },
      new List<IEffectSpec>() { }
    ) {
    }

    /// <summary>
    /// Simple constructor with a single cost and effect.
    /// </summary>
    public Useable(TargetScope scope, IEffectSpec effect, IEffectSpec cost) : this(
      scope,
      new List<IEffectSpec>() { effect },
      new List<IEffectSpec>() { cost }
    ) {
    }

    /// <summary>
    /// All purpose constructor with zero or more effects and costs.
    /// </summary>
    public Useable(TargetScope scope, List<IEffectSpec> effects, List<IEffectSpec> costs) {
      // Multi-effect constructor
      this.Scope = scope;
      this.effects = effects;
      this.costs = costs;
    }

    public void Load(IGameDataReader reader) {
      this.Scope = (TargetScope)reader.ReadInt();

      int costCount = reader.ReadInt();
      this.costs = new List<IEffectSpec>(costCount);
      for (int i = 0; i < costCount; i++) {
        EffectSpec cost = new();
        cost.Load(reader);
        this.costs.Add(cost);
      }

      int effectsCount = reader.ReadInt();
      this.effects = new List<IEffectSpec>(effectsCount);
      for (int i = 0; i < effectsCount; i++) {
        EffectSpec effect = new();
        effect.Load(reader);
        this.effects.Add(effect);
      }
    }

    public void Save(IGameDataWriter writer) {
      writer.Write((int)this.Scope);

      writer.Write(this.costs.Count);
      for (int i = 0; i < this.costs.Count; i++) {
        this.costs[i].Save(writer);
      }

      writer.Write(this.effects.Count);
      for (int i = 0; i < this.effects.Count; i++) {
        this.effects[i].Save(writer);
      }

    }
  }
}
