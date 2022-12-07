namespace Tests.Server {
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common.GameEvents;
  using URF.Server;
  using URF.Server.GameState;
  using URF.Common;

  public class GameStateTests {


    private GameState gameState;

    [SetUp]
    public void SetUp() {
      this.gameState = new GameState(5, 5);
    }

    [Test]
    public void Constructor_Should_RejectNonPositiveDimensions() {
      _ = Assert.Throws<ArgumentException>(() => new GameState(-1, 1));
      _ = Assert.Throws<ArgumentException>(() => new GameState(1, -1));
      _ = Assert.Throws<ArgumentException>(() => new GameState(0, 1));
      _ = Assert.Throws<ArgumentException>(() => new GameState(1, 0));
    }

    [Test]
    public void CreateEntity_ShouldNot_AllowCreatingNullEntity() {
      _ = Assert.Throws<ArgumentNullException>(() => this.gameState.CreateEntity(null));
    }

    [Test]
    public void CreateEntity_ShouldNot_AllowCreatingTheSameEntity() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      _ = Assert.Throws<ArgumentException>(() => this.gameState.CreateEntity(entity));
    }

    [Test]
    public void CreateEntity_Should_AllowCreatingTwoDifferentEntities() {
      var entityA = new Entity();
      var entityB = new Entity();
      this.gameState.CreateEntity(entityA);
      this.gameState.CreateEntity(entityB);
    }

    [Test]
    public void CreateEntity_Should_CreateAnEntitySuccessfully() {
      this.gameState.CreateEntity(new Entity());
    }

    // Test handler to exposed caught events.
    private class EntityCreatedListener : BaseGameEventChannel {
      public List<EntityCreated> Events {
        get;
      } = new();
      public override void HandleEntityCreated(EntityCreated entityCreated) {
        this.Events.Add(entityCreated);
      }
    }

    [Test]
    public void CreateEntity_Should_EmitOneEntityCreatedEvent() {
      var handler = new EntityCreatedListener();
      handler.Listen(this.gameState);

      var entity = new Entity();
      this.gameState.CreateEntity(entity);

      Assert.That(handler.Events.Count, Is.EqualTo(1));
    }

    [Test]
    public void CreateEntity_Should_EmitAnEntityCreatedEventThatContainsTheCreatedEntity() {
      var handler = new EntityCreatedListener();
      handler.Listen(this.gameState);

      var entity = new Entity();
      this.gameState.CreateEntity(entity);

      Assert.That(handler.Events.Single().Entity, Is.EqualTo(entity));
    }

    [Test]
    public void GetAllEntities_Should_BeginEmpty() {
      Assert.That(this.gameState.GetAllEntities().Count, Is.EqualTo(0));
    }

    [Test]
    public void GetAllEntities_Should_ContainCreatedEntities() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);

      Assert.That(this.gameState.GetAllEntities().Single(), Is.EqualTo(entity));
    }

    [Test]
    public void GetAllEntities_ShouldNot_ContainDeletedEntities() {
      var entityA = new Entity();
      this.gameState.CreateEntity(entityA);
      var entityB = new Entity();
      this.gameState.CreateEntity(entityB);

      this.gameState.DeleteEntity(entityA);
      Assert.That(this.gameState.GetAllEntities().Single(), Is.EqualTo(entityB));
    }

    [Test]
    public void PlaceEntityOnMap_Should_RejectDetachedEntities() {
      var entity = new Entity();
      _ = Assert.Throws<EntityDetachedException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(1, 1))
      );
    }

    [Test]
    public void PlaceEntityOnMap_Should_RejectNullEntities() {
      _ = Assert.Throws<ArgumentNullException>(
        () => this.gameState.PlaceEntityOnMap(null, new Position(1, 1))
      );
    }

    [Test]
    public void PlaceEntityOnMap_Should_RejectIllegalPositions() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(-1, 0))
      );
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(0, -1))
      );
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(-1, -1))
      );
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(5, 0))
      );
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(0, 5))
      );
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(5, 5))
      );
    }

    [Test]
    public void PlaceEntityOnMap_Should_RejectMovingEntityViaPlace() {
      // debateable whether this is the correct behavior or not, but probably indicates
      // an error in logic if it occurs, so we'll keep it until we have a reason not to
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));
      _ = Assert.Throws<ArgumentException>(
        () => this.gameState.PlaceEntityOnMap(entity, new Position(1, 1))
      );
    }

    [Test]
    public void PlaceEntityOnMap_Should_BeInTheCellWhereItWasPlaced() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));
      Cell cell = this.gameState.GetCell(new Position(0, 0));
      Assert.That(cell.Contents.Contains(entity));
    }

    [Test]
    public void PlaceEntityOnMap_Should_BeAllowedAfterRemovingTheEntityFromMap() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));
      this.gameState.RemoveEntityFromMap(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));
      Cell cell = this.gameState.GetCell(new Position(0, 0));
      Assert.That(cell.Contents.Contains(entity));
    }

    [Test]
    public void PlaceEntityOnMap_ShouldNot_BeInACellWhereItWasNotPlaced() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));
      for (int i = 0; i < this.gameState.MapSize.X; i++) {
        for (int j = 0; j < this.gameState.MapSize.Y; j++) {
          if (i == 0 && j == 0) {
            continue;
          } else {
            Cell cell = this.gameState.GetCell(new Position(i, j));
            Assert.That(!cell.Contents.Contains(entity));
          }
        }
      }
    }

    [Test]
    public void PlaceEntityOnMap_ShouldNot_OverwritePriorPlacements() {
      var entityA = new Entity();
      var entityB = new Entity();

      this.gameState.CreateEntity(entityA);
      this.gameState.CreateEntity(entityB);
      this.gameState.PlaceEntityOnMap(entityA, new Position(0, 0));
      this.gameState.PlaceEntityOnMap(entityB, new Position(0, 0));

      Cell cell = this.gameState.GetCell(new Position(0, 0));
      Assert.That(cell.Contents.Contains(entityA));
      Assert.That(cell.Contents.Contains(entityB));
    }

    private class EntityLocationChangedListener : BaseGameEventChannel {
      public List<EntityLocationChanged> Events {
        get;
      } = new();
      public override void HandleEntityLocationChanged(
        EntityLocationChanged entityLocationChanged) {
        this.Events.Add(entityLocationChanged);
      }
    }

    [Test]
    public void PlaceEntityOnMap_Should_EmitEntityPlaced() {
      var entity = new Entity();
      var listener = new EntityLocationChangedListener();
      listener.Listen(this.gameState);

      this.gameState.CreateEntity(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));

      EntityLocationChanged emitted = listener.Events.Single();
      Assert.That(emitted.Entity, Is.EqualTo(entity));
      Assert.That(emitted.SubType, Is.EqualTo(EntityLocationChanged.EventSubType.Placed));
      Assert.That(emitted.NewPosition, Is.EqualTo(new Position(0, 0)));
    }

    [Test]
    public void PlaceEntityOnMap_Should_EmitEntityPlacedInTheCorrectSequence() {
      var entityA = new Entity();
      var entityB = new Entity();
      var listener = new EntityLocationChangedListener();
      listener.Listen(this.gameState);

      this.gameState.CreateEntity(entityA);
      this.gameState.CreateEntity(entityB);
      this.gameState.PlaceEntityOnMap(entityA, new Position(0, 0));
      this.gameState.PlaceEntityOnMap(entityB, new Position(1, 1));

      List<EntityLocationChanged> emitted = listener.Events;
      Assert.That(emitted[0].Entity, Is.EqualTo(entityA));
      Assert.That(emitted[0].NewPosition, Is.EqualTo(new Position(0, 0)));
      Assert.That(emitted[1].Entity, Is.EqualTo(entityB));
      Assert.That(emitted[1].NewPosition, Is.EqualTo(new Position(1, 1)));
    }

    [Test]
    public void RemoveEntityFromMap_Should_RemoveTheEntityFromTheMap() {
      var entity = new Entity();
      this.gameState.CreateEntity(entity);
      this.gameState.PlaceEntityOnMap(entity, new Position(0, 0));
      this.gameState.RemoveEntityFromMap(entity);
      Cell cell = this.gameState.GetCell(new Position(0, 0));
      Assert.That(cell.Contents, Is.Empty);
    }

    [Test]
    public void RemoveEntityFromMap_Should_RejectNullEntities() {
      _ = Assert.Throws<ArgumentNullException>(() => this.gameState.RemoveEntityFromMap(null));
    }

    [Test]
    public void RemoveEntityFromMap_Should_RejectDetachedEntities() {
      var entity = new Entity();
      _ = Assert.Throws<EntityDetachedException>(() => this.gameState.RemoveEntityFromMap(entity));
    }

    [Test]
    public void RemoveEntityFromMap_ShouldNot_RemoveOtherEntitiesFromTheMap() {
      var entityA = new Entity();
      var entityB = new Entity();
      this.gameState.CreateEntity(entityA);
      this.gameState.CreateEntity(entityB);

      this.gameState.PlaceEntityOnMap(entityA, new Position(0, 0));
      this.gameState.PlaceEntityOnMap(entityB, new Position(0, 0));

      this.gameState.RemoveEntityFromMap(entityA);
      Cell cell = this.gameState.GetCell(new Position(0, 0));
      Assert.That(cell.Contents.Contains(entityB));
    }
  }

}
