namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Effects;

  public class EffectEvent : EventArgs, IGameEvent {

    public enum EffectEventStep {
      // Hypothetically apply the effect to see if it can be fully executed.
      Queried,
      // The hypothetical cost could be full applied
      Confirmed,
      // The hypothetical cost could not be fully applied
      Denied,
      // The effect has been created but not applied.
      Created,
      // The effect has been applied and should be considered done.
      Applied
    }

    public IEffect Effect {
      get;
    }

    public EffectEventStep Step {
      get;
    }

    public EffectEvent Applied => new(
      this.Effect,
      EffectEventStep.Applied
    );

    public EffectEvent(IEffect effect) {
      this.Effect = effect;
      this.Step = EffectEventStep.Created;
    }

    public EffectEvent(IEffect effect, EffectEventStep step) {
      this.Effect = effect;
      this.Step = step;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEffectEvent(this);
    }
  }
}
