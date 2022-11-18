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
      void builder(IEntity entity) => wasInvoked = true;
      this.entityFactory.RegisterBlueprint("testBlueprint", builder);
      _ = this.entityFactory.Get("testBlueprint");
      Assert.That(wasInvoked, "Entity factory should invoke the test builder.");
    }

    [Test]
    public void EntityFactory_Should_OnlyInvokeCorrectBuilder() {
      bool firstInvoked = false;
      bool secondInvoked = false;
      void first(IEntity entity) => firstInvoked = true;
      this.entityFactory.RegisterBlueprint("first", first);
      void second(IEntity entity) => secondInvoked = true;
      this.entityFactory.RegisterBlueprint("second", second);

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
      void first(IEntity entity) => firstInvoked = true;
      this.entityFactory.RegisterBlueprint("first", first);
      void second(IEntity entity) => secondInvoked = true;
      this.entityFactory.RegisterBlueprint("second", second);

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

    private class ComponentA : BaseComponent {
      public override void Load(GameDataReader reader) => throw new NotImplementedException();
      public override void Save(GameDataWriter writer) => throw new NotImplementedException();
    }

    private class ComponentB : BaseComponent {
      public override void Load(GameDataReader reader) => throw new NotImplementedException();
      public override void Save(GameDataWriter writer) => throw new NotImplementedException();
    }

    private class ComponentC : BaseComponent {
      public override void Load(GameDataReader reader) => throw new NotImplementedException();
      public override void Save(GameDataWriter writer) => throw new NotImplementedException();
    }

    private class FakeEntityType : IEntity {

      public int ID {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
      }
      public bool BlocksSight {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
      }
      public bool IsVisible {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
      }

      public void AddComponent(BaseComponent component) => Components.Add(component);

      public TComponentType GetComponent<TComponentType>() where TComponentType : BaseComponent
      => throw new NotImplementedException();
      public void Load(GameDataReader reader) => throw new NotImplementedException();
      public void Save(GameDataWriter writer) => throw new NotImplementedException();
    }

    [Test]
    public void EntityFactory_Should_AddTheSpecifiedComponents() {
      EntityFactory<FakeEntityType> factory = new();
      factory.UpdateEntitySpec(new List<Type> { typeof(ComponentA), typeof(ComponentB) });
      IEntity entity = factory.Get();
      Assert.That(Components.Any(x => x is ComponentA));
      Assert.That(Components.Any(x => x is ComponentB));
      Assert.That(!Components.Any(x => x is ComponentC));

    }
  }
}
