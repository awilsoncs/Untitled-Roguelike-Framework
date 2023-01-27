namespace URF.Common.Useables {
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Persistence;
  using URF.Server.Resolvables;

  /// <summary>
  /// A useable, e.g. an item or ability.
  /// </summary>
  public interface IUseable : IPersistableObject {

    IEnumerable<IEffectSpec> Costs {
      get;
    }

    /// <summary>
    /// The affected scope of this action, e.g. self, one creature, one position.
    /// </summary>
    TargetScope Scope {
      get;
    }

    /// <summary>
    /// A series of effects to be applied to each target.
    /// </summary>
    IEnumerable<IEffectSpec> Effects {
      get;
    }


  }
}
