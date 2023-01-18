namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Effects;

  public class EffectEvent : EventArgs, IGameEvent {

    // todo include a list of affected
    public enum EffectEventStep {
      Created,
      Applied
    }

    public IEffect Effect {
      get;
    }

    public EffectEventStep Step {
      get;
    }

    public EffectEvent(IEffect effect, EffectEventStep step) {
      this.Effect = effect;
      this.Step = step;
    }

    public static EffectEvent Created(IEffect effect) {
      return new EffectEvent(effect, EffectEventStep.Created);
    }

    public static EffectEvent Applied(IEffect effect) {
      return new EffectEvent(effect, EffectEventStep.Applied);
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEffectEvent(this);
    }
  }
}
