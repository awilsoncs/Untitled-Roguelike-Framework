namespace Tests.Server {
  using NUnit.Framework;
  using URF.Server;
  using URF.Server.EntityFactory;

  public class EntityFactoryTests {

    private EntityFactory<Entity> entityFactory;

    [SetUp]
    public void Setup() {
      this.entityFactory = new EntityFactory<Entity>();
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
  }
}
