namespace URF.Server.Useables {
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.Persistence;
  using URF.Common.Useables;
  using URF.Server.Resolvables;

  /// <summary>
  /// A useable, e.g. an item or ability.
  /// </summary>
  public class Useable : IUseable {

    /// <inheritdoc />
    public TargetScope Scope {
      get; set;
    }

    /// <inheritdoc />
    public IEnumerable<IEffect> Effects => this.effects;
    private List<IEffect> effects;

    public Useable() {
      // We need the default constructor to support serialization patterns.
    }

    public Useable(TargetScope scope, IEffect effect) {
      // Simple constructor
      this.Scope = scope;
      this.effects = new List<IEffect>() { effect };
    }

    public Useable(TargetScope scope, List<IEffect> effects) {
      // Multi-effect constructor
      this.Scope = scope;
      this.effects = effects;
    }

    public void Load(IGameDataReader reader) {
      this.Scope = (TargetScope)reader.ReadInt();

      int effectsCount = reader.ReadInt();
      this.effects = new List<IEffect>(effectsCount);
      for (int i = 0; i < reader.ReadInt(); i++) {
        Effect effect = new();
        effect.Load(reader);
        this.effects[i] = effect;
      }

    }

    public void Save(IGameDataWriter writer) {
      writer.Write((int)this.Scope);

      writer.Write(this.effects.Count);
      for (int i = 0; i < this.effects.Count; i++) {
        this.effects[i].Save(writer);
      }

    }

    public IResolvable UsedBy(IEntity entity) {
      return new Resolvable(entity, this.Scope, this.Effects);
    }
  }
}
