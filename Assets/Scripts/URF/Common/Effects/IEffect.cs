namespace URF.Common.Effects {
  using URF.Common.Entities;

  public interface IEffect {

    IEntity Agent {
      get;
    }

    IEntity Source {
      get;
    }

    IEntity Affected {
      get;
    }

    IEffectSpec Spec {
      get;
    }
  }
}
