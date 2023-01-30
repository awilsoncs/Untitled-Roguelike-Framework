namespace Tests.Server.RulesSystems {
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server;
  using URF.Server.Effects;
  using URF.Server.Resolvables;
  using URF.Server.RulesSystems;
  using URF.Server.Useables;

  public class ResolvableSystemTests {

    [Test]
    public void ResolvableSystem_Should_EmitTargetRequestEventOneCreature() {

      Entity agent = new();
      Entity source = new();
      Entity target = new();
      agent.VisibleEntities.Add(target);

      Useable useable = new(TargetScope.OneCreature, new EffectSpec(EffectType.RestoreHealth, 0));

      Resolvable resolvable = new(agent, source, useable);
      ResolvableEvent ev = new(resolvable);
      ResolvableSystem resolvableSystem = new();

      List<IGameEvent> events = new();
      resolvableSystem.GameEvent += (sender, gameEvent) => events.Add(gameEvent);

      resolvableSystem.HandleResolvableEvent(ev);

      Assert.That(events.Count, Is.EqualTo(1));
      IGameEvent outEvent = events[0];
      Assert.That(events[0], Is.AssignableTo(typeof(TargetEvent)));
      var targetEvent = (TargetEvent)events[0];
      Assert.That(targetEvent.Method, Is.EqualTo(TargetEvent.TargetEventMethod.Request));
      Assert.That(targetEvent.Resolvable, Is.EqualTo(resolvable));
      Assert.That(targetEvent.SelectedTargets, Is.Empty);
      Assert.That(targetEvent.Targets, Is.EquivalentTo(new List<IEntity> { target }));
    }

    // todo no legal targets

    [Test]
    public void ResolvableSystem_Should_ResolvableEventOnSelfTargets() {
      // When the system receives a ResolvableEvent with a self targeting Useable, it should select
      // the user as the target and skip to cost determination.

      Entity agent = new();
      Entity source = new();
      Entity target = new();
      agent.VisibleEntities.Add(target);
      Useable useable = new(TargetScope.Self, new EffectSpec(EffectType.RestoreHealth, 0));
      Resolvable resolvable = new(agent, source, useable);
      ResolvableEvent ev = new(resolvable);
      ResolvableSystem resolvableSystem = new();

      List<IGameEvent> events = new();
      resolvableSystem.GameEvent += (sender, gameEvent) => events.Add(gameEvent);
      resolvableSystem.HandleResolvableEvent(ev);

      Assert.That(events.Count, Is.EqualTo(1));
      IGameEvent outEvent = events[0];
      Assert.That(events[0], Is.AssignableTo(typeof(ResolvableEvent)));
      var resolvableEvent = (ResolvableEvent)events[0];
      Assert.That(resolvableEvent.Resolvable, Is.EqualTo(resolvable));
      Assert.That(
        resolvableEvent.Step, Is.EqualTo(ResolvableEvent.ResolvableEventStep.CostDetermination));
      Assert.That(
        resolvableEvent.Resolvable.ResolvedTargets, Is.EquivalentTo(new List<IEntity> { agent }));
    }

  }
}
