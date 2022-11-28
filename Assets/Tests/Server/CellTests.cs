namespace Tests.Backend {
  using NUnit.Framework;
  using URF.Server;
  using URF.Server.GameState;
  using URF.Server.RulesSystems;

  public class CellTests {
    private Cell cell;

    [SetUp]
    public void Setup() {
      this.cell = new Cell();
    }

    [Test]
    public void CellsBeginEmpty() {
      Assert.That(this.cell.Contents, Is.Empty);
    }

    [Test]
    public void CellsCanGainContents() {
      this.cell.PutContents(new Entity());
      const int expectedCount = 1;
      Assert.That(this.cell.Contents, Has.Count.EqualTo(expectedCount));
      this.cell.PutContents(new Entity());
      const int newExpectedCount = 2;
      Assert.That(this.cell.Contents, Has.Count.EqualTo(newExpectedCount));
    }

    [Test]
    public void EntityCanBeRemovedFromCell() {
      var entity = new Entity();
      this.cell.PutContents(entity);
      this.cell.RemoveEntity(entity);
      Assert.That(this.cell.Contents, Is.Empty);
    }

    [Test]
    public void RemovingAnEntityOnlyRemovesThatEntity() {
      var entity1 = new Entity();
      var entity2 = new Entity();
      this.cell.PutContents(entity1);
      this.cell.PutContents(entity2);

      this.cell.RemoveEntity(entity1);
      const int expectedCount = 1;
      Assert.That(this.cell.Contents, Has.Count.EqualTo(expectedCount));
      Assert.That(this.cell.Contents, Does.Contain(entity2));
    }

    [Test]
    public void EntityCannotBeAddedTwice() {
      var entity = new Entity();
      this.cell.PutContents(entity);
      this.cell.PutContents(entity);
      const int expectedCount = 1;
      Assert.That(this.cell.Contents, Has.Count.EqualTo(expectedCount));
    }

    [Test]
    public void EmptyCellShouldBeTransparent() {
      Assert.That(this.cell.IsTransparent);
    }

    [Test]
    public void CellWithTransparentEntityShouldBeTransparent() {
      var entity = new Entity { BlocksSight = false };
      this.cell.PutContents(entity);
      Assert.That(this.cell.IsTransparent);
    }

    [Test]
    public void CellWithOpaqueEntityShouldNotBeTransparent() {
      var entity = new Entity { BlocksSight = true };
      this.cell.PutContents(entity);
      Assert.That(!this.cell.IsTransparent);
    }

    [Test]
    public void CellWithOpaqueAndTransparentContentsShouldNotBeTransparent() {
      var entity1 = new Entity { BlocksSight = false };
      this.cell.PutContents(entity1);
      var entity2 = new Entity { BlocksSight = true };
      this.cell.PutContents(entity2);
      Assert.That(!this.cell.IsTransparent);
    }

    [Test]
    public void EmptyCellShouldByPassable() {
      Assert.That(this.cell.IsPassable);
    }

    [Test]
    public void CellWithOnlyPassableEntityShouldBePassable() {
      var entity = new Entity();
      entity.AddComponent(new Movement { BlocksMove = false });
      this.cell.PutContents(entity);
      Assert.That(this.cell.IsPassable);
    }

    [Test]
    public void CellWithOnlyBlockingEntityShouldNotBePassable() {
      var entity = new Entity();
      entity.AddComponent(new Movement { BlocksMove = true });
      this.cell.PutContents(entity);
      Assert.That(!this.cell.IsPassable);
    }

    [Test]
    public void CellWithBlockingAndPassableContentsShouldNotBePassable() {
      var entity1 = new Entity();
      entity1.AddComponent(new Movement { BlocksMove = false });
      this.cell.PutContents(entity1);

      var entity2 = new Entity();
      entity2.AddComponent(new Movement { BlocksMove = true });
      this.cell.PutContents(entity2);
      Assert.That(!this.cell.IsPassable);
    }
  }
}
