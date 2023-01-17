namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.GameEvents;

  public class EffectsSystem : BaseRulesSystem, IEventForwarder {
    public void ForwardEvent(object _, IGameEvent ev) {
      this.OnGameEvent(ev);
    }

    public override void HandleEffectEvent(EffectEvent ev) {
      if (ev.Step != EffectEvent.EffectEventStep.Created) {
        return;
      }
      ev.Effect.Apply(this, this.GameState);
      this.OnGameEvent(EffectEvent.Applied(ev.Effect));
    }
  }
}
