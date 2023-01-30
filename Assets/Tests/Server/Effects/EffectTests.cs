namespace Tests.Server.Effects {
  using System;
  using NUnit.Framework;
  using URF.Server;
  using URF.Server.Effects;

  public class EffectTests {

    [Test]
    public void Effect_Should_HaveGivenValues() {
      Entity agent = new();
      Entity source = new();
      Entity affected = new();
      EffectSpec spec = new();

      Effect effect = new(agent, source, affected, spec);

      Assert.That(effect.Agent, Is.EqualTo(agent));
      Assert.That(effect.Source, Is.EqualTo(source));
      Assert.That(effect.Affected, Is.EqualTo(affected));
      Assert.That(effect.Spec, Is.EqualTo(spec));
    }

    [Test]
    public void Effect_Should_NotAllowNullValues() {
      Entity agent = new();
      Entity source = new();
      Entity affected = new();
      EffectSpec spec = new();

      _ = Assert.Throws<ArgumentNullException>(() => new Effect(null, source, affected, spec));
      _ = Assert.Throws<ArgumentNullException>(() => new Effect(agent, null, affected, spec));
      _ = Assert.Throws<ArgumentNullException>(() => new Effect(agent, source, null, spec));
      _ = Assert.Throws<ArgumentNullException>(() => new Effect(agent, source, affected, null));
    }

  }
}
