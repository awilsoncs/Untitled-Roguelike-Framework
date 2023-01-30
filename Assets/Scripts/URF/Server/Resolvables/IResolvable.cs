namespace URF.Server.Resolvables {
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Common.Useables;

  /// <summary>
  /// Resolvables represent the concept of a live instance of an activated ability from a spell
  /// or item as it's being activated. They contain information about the activation agent, legal
  /// targets, costs, etc. After the Resolvable is resolved, it can be queried for EffectEvents.
  /// </summary>
  public interface IResolvable {

    /// <summary>
    /// The IEntity source agent of the resolvable. This is the Intelligence that performed the
    /// action, not the item or ability that provided the action.
    /// </summary>
    IEntity Agent {
      get;
    }

    /// <summary>
    /// The IEntity that provided the capability to create this item, e.g. an item or the
    /// spell caster.
    /// </summary>
    /// <value></value>
    IEntity Source {
      get;
    }

    IUseable Useable {
      get;
    }

    IEnumerable<IEntity> LegalTargets {
      get;
    }

    IEnumerable<IEntity> ResolvedTargets {
      get;
    }

    void AddLegalTarget(IEntity entity);

    void ResolveTarget(IEntity target);
  }
}
