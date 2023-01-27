namespace Tests.Server.Effects {
  using System;
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common.Entities;
  using URF.Server;
  using URF.Server.Resolvables;
  using URF.Server.Useables;

  public class ResolvableTests {

    private Resolvable resolvable;

    [SetUp]
    public void Setup() {
      Entity agent = new();
      Entity source = new();
      Useable useable = new() {
        Scope = TargetScope.OneCreature
      };
      this.resolvable = new(agent, source, useable);
    }

    [Test]
    public void Resolvable_Should_HaveGivenValues() {
      Entity agent = new();
      Entity source = new();
      Useable useable = new();

      Resolvable resolvable = new(agent, source, useable);

      Assert.That(resolvable.Agent, Is.EqualTo(agent));
      Assert.That(resolvable.Source, Is.EqualTo(source));
      Assert.That(resolvable.Useable, Is.EqualTo(useable));
    }

    [Test]
    public void Resolvable_Should_NotAllowNullValues() {
      Entity agent = new();
      Entity source = new();
      Useable useable = new();

      _ = Assert.Throws<ArgumentNullException>(() => new Resolvable(null, source, useable));
      _ = Assert.Throws<ArgumentNullException>(() => new Resolvable(agent, null, useable));
      _ = Assert.Throws<ArgumentNullException>(() => new Resolvable(agent, source, null));
    }

    [Test]
    public void Resolvable_Should_BeginWithEmptyLegalTargets() {
      Assert.That(this.resolvable.LegalTargets, Is.Empty);
    }

    [Test]
    public void Resolvable_Should_BeginWithEmptyResolvedTargets() {
      Assert.That(this.resolvable.LegalTargets, Is.Empty);
    }

    [Test]
    public void Resolvable_Should_RememberLegalTargets() {
      Entity legalTarget = new();
      this.resolvable.AddLegalTarget(legalTarget);
      Assert.That(
        this.resolvable.LegalTargets,
        Is.EquivalentTo(new List<IEntity>() { legalTarget }));
    }

    [Test]
    public void Resolvable_ShouldNot_AcceptNullLegalTarget() {
      _ = Assert.Throws<ArgumentNullException>(() => this.resolvable.AddLegalTarget(null));
    }

    [Test]
    public void Resolvable_ShouldNot_AllowAddingSameLegalTargetTwice() {
      Entity legalTarget = new();
      this.resolvable.AddLegalTarget(legalTarget);
      _ = Assert.Throws<ArgumentException>(() => this.resolvable.AddLegalTarget(legalTarget));
    }

    [Test]
    public void Resolvable_Should_RememberResolvedTargets() {
      Entity legalTarget = new();
      this.resolvable.AddLegalTarget(legalTarget);
      this.resolvable.ResolveTarget(legalTarget);
      Assert.That(
        this.resolvable.ResolvedTargets,
        Is.EquivalentTo(new List<IEntity>() { legalTarget }));
    }

    [Test]
    public void Resolvable_ShouldNot_AllowResolvingNullTarget() {
      Entity legalTarget = new();
      this.resolvable.AddLegalTarget(legalTarget);
      _ = Assert.Throws<ArgumentNullException>(() => this.resolvable.ResolveTarget(null));
    }

    [Test]
    public void Resolvable_ShouldNot_AllowResolvingSameTargetTwice() {
      Entity legalTarget = new();
      this.resolvable.AddLegalTarget(legalTarget);
      this.resolvable.ResolveTarget(legalTarget);
      _ = Assert.Throws<ArgumentException>(() => this.resolvable.ResolveTarget(legalTarget));
    }

    [Test]
    public void Resolvable_ShouldNot_Allow_ResolvingIllegalTarget() {
      Entity legalTarget = new();
      Entity illegalTarget = new();

      this.resolvable.AddLegalTarget(legalTarget);
      _ = Assert.Throws<ArgumentException>(() => this.resolvable.ResolveTarget(illegalTarget));
    }

    [Test]
    public void Resolvable_Should_EnforceSelfScope() {
      Entity agent = new();
      Entity source = new();
      Useable useable = new() {
        Scope = TargetScope.Self
      };

      Resolvable resolvable = new(agent, source, useable);
      _ = Assert.Throws<ArgumentException>(() => resolvable.AddLegalTarget(source));
    }

    [Test]
    public void Resolvable_Should_EnforceOneCreatureScope() {
      Entity agent = new();
      Entity source = new();
      Useable useable = new() {
        Scope = TargetScope.OneCreature
      };

      Resolvable resolvable = new(agent, source, useable);
      resolvable.AddLegalTarget(agent);
      resolvable.AddLegalTarget(source);
      resolvable.ResolveTarget(agent);

      _ = Assert.Throws<ArgumentException>(() => resolvable.ResolveTarget(source));
    }

  }
}
