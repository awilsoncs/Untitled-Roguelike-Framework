namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class EffectEvent : EventArgs, IGameEvent {

    public enum EffectEventStep {
      Created,
      Applied
    }

    public enum EffectType {
      RestoreHealth
    }

    public EffectType Method {
      get;
    }

    public int Magnitude {
      get;
    }

    public IEntity Affected {
      get;
    }

    public EffectEventStep Step {
      get;
    }

    public EffectEvent Applied => new(
      this.Method,
      this.Magnitude,
      this.Affected,
      EffectEventStep.Applied
    );

    public EffectEvent(EffectType method, int magnitude, IEntity affected) {
      this.Method = method;
      this.Magnitude = magnitude;
      this.Affected = affected;
      this.Step = EffectEventStep.Created;
    }

    private EffectEvent(EffectType method, int magnitude, IEntity affected, EffectEventStep step) {
      this.Method = method;
      this.Magnitude = magnitude;
      this.Affected = affected;
      this.Step = step;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEffectEvent(this);
    }
  }
}
