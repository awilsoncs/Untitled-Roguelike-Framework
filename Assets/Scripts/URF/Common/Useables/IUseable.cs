namespace URF.Common.Useables {
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.Persistence;
  using URF.Server.Resolvables;

  /// <summary>
  /// A useable, e.g. an item or ability.
  /// </summary>
  public interface IUseable : IPersistableObject {
    TargetScope Scope {
      get;
    }

    IEnumerable<IEffect> Effects {
      get;
    }

    IResolvable UsedBy(IEntity entity);
  }
}
