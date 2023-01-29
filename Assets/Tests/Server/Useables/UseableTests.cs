namespace Tests.Server.Useables {
  using System;
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common.Effects;
  using URF.Server.Effects;
  using URF.Server.Useables;

  public class UseableTests {
    [Test]
    public void Useable_Should_DefaultToInvalidScope() {
      Useable useable = new();
      Assert.That(useable.Scope, Is.EqualTo(TargetScope.Invalid));
    }

    [Test]
    public void Useable_Should_HaveGivenValues() {
      EffectSpec costs = new(EffectType.DamageHealth, 1);
      EffectSpec effects = new(EffectType.RestoreHealth, 1);
      Useable useable = new(TargetScope.Self, costs, effects);
      Assert.That(useable.Scope, Is.EqualTo(TargetScope.Self));
      Assert.That(useable.Costs, Is.EqualTo(new List<IEffectSpec> { costs }));
      Assert.That(useable.Effects, Is.EqualTo(new List<IEffectSpec> { effects }));
    }

    [Test]
    public void Resolvable_Should_NotAllowNullValues() {

      List<IEffectSpec> costs = new() {
        new EffectSpec(EffectType.DamageHealth, 1)
      };
      List<IEffectSpec> effects = new() {
        new EffectSpec(EffectType.RestoreHealth, 1)
      };

      _ = Assert.Throws<ArgumentNullException>(() => new Useable(TargetScope.Self, null, effects));
      _ = Assert.Throws<ArgumentNullException>(() => new Useable(TargetScope.Self, costs, null));

    }
  }
}
