namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Effects;
  using URF.Common.Entities;

  public class EffectEvent : EventArgs, IGameEvent {

    public enum EffectEventStep {
      Created,
      Applied
    }

    public IEffect Effect {
      get;
    }

    public IEntity Affected {
      get;
    }

    public EffectEventStep Step {
      get;
    }

    public EffectEvent Applied => new(
      this.Effect,
      this.Affected,
      EffectEventStep.Applied
    );

    public EffectEvent(IEffect effect, IEntity affected) {
      this.Effect = effect;
      this.Affected = affected;
      this.Step = EffectEventStep.Created;
    }

    private EffectEvent(IEffect effect, IEntity affected, EffectEventStep step) {
      this.Effect = effect;
      this.Affected = affected;
      this.Step = step;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEffectEvent(this);
    }
  }
}
