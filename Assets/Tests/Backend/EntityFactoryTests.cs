namespace Tests.Server {
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common.Entities;
  using URF.Common.Persistence;
  using URF.Server;
  using URF.Server.EntityFactory;

  public class EntityFactoryTests {

    private EntityFactory<Entity> entityFactory;
    private static readonly List<BaseComponent> Components = new();



    [SetUp]
    public void Setup() {
      this.entityFactory = new EntityFactory<Entity>();
      Components.Clear();
    }

    [Test]
    public void EntityFactory_Should_InvokeBuilder() {
      bool wasInvoked = false;
      this.entityFactory.RegisterBlueprint("testBlueprint", _ => wasInvoked = true);
      _ = this.entityFactory.Get("testBlueprint");
      Assert.That(wasInvoked, "Entity factory should invoke the test builder.");
    }

    [Test]
    public void EntityFactory_Should_OnlyInvokeCorrectBuilder() {
      bool firstInvoked = false;
      bool secondInvoked = false;
      this.entityFactory.RegisterBlueprint("first", _ => firstInvoked = true);
      this.entityFactory.RegisterBlueprint("second", _ => secondInvoked = true);

      _ = this.entityFactory.Get("first");
      Assert.That(
        firstInvoked,
        "EntityFactory should invoke first builder when getting first blueprint."
      );
      Assert.That(
        !secondInvoked,
        "EntityFactory should not invoke second builder when getting first blueprint."
      );

      firstInvoked = false;
      secondInvoked = false;

      _ = this.entityFactory.Get("second");
      Assert.That(
        !firstInvoked,
        "EntityFactory should not invoke first builder when getting second blueprint.");
      Assert.That(
        secondInvoked,
        "EntityFactory should invoke second builder when getting second blueprint.");
    }

    [Test]
    public void EntityFactory_ShouldNot_InvokeBuildersForBaseEntity() {
      bool firstInvoked = false;
      bool secondInvoked = false;
      this.entityFactory.RegisterBlueprint("first", _ => firstInvoked = true);
      this.entityFactory.RegisterBlueprint("second", _ => secondInvoked = true);

      _ = this.entityFactory.Get();
      Assert.That(
        !firstInvoked,
        "EntityFactory should not invoke first builder when getting base Entity."
      );
      Assert.That(
        !secondInvoked,
        "EntityFactory should not invoke second builder when getting base Entity."
      );
    }

    private sealed class ComponentA : BaseComponent {
      public override void Load(GameDataReader reader) => throw new NotSupportedException();
      public override void Save(GameDataWriter writer) => throw new NotSupportedException();
    }

    private sealed class ComponentB : BaseComponent {
      public override void Load(GameDataReader reader) => throw new NotSupportedException();
      public override void Save(GameDataWriter writer) => throw new NotSupportedException();
    }

    private sealed class ComponentC : BaseComponent {
      public override void Load(GameDataReader reader) => throw new NotSupportedException();
      public override void Save(GameDataWriter writer) => throw new NotSupportedException();
    }

    private sealed class FakeEntityType : IEntity {

      public int ID {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
      }
      public bool BlocksSight {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
      }
      public bool IsVisible {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
      }

      public void AddComponent(BaseComponent component) => Components.Add(component);

      public TComponentType GetComponent<TComponentType>() where TComponentType : BaseComponent
      => throw new NotImplementedException();
      public void Load(GameDataReader reader) => throw new NotSupportedException();
      public void Save(GameDataWriter writer) => throw new NotSupportedException();
    }

    [Test]
    public void EntityFactory_Should_AddTheSpecifiedComponents() {
      EntityFactory<FakeEntityType> factory = new();
      factory.UpdateEntitySpec(new List<Type> { typeof(ComponentA), typeof(ComponentB) });
      _ = factory.Get();
      Assert.That(Components.Any(x => x is ComponentA));
      Assert.That(Components.Any(x => x is ComponentB));
      Assert.That(!Components.Any(x => x is ComponentC));

    }
  }
}
