using NUnit.Framework;
using System.Collections.Generic;
using URFCommon;

public class CellTests
{
    Cell cell;

    [SetUp]
    public void Setup() {
        cell = new Cell();
    }

    [Test]
    public void CellsBeginEmpty() {
        Assert.That(cell.Contents, Is.Empty);
    }

    [Test]
    public void CellsCanGainContents() {
        cell.PutContents(new Entity());
        Assert.That(cell.Contents, Has.Count.EqualTo(1));
        cell.PutContents(new Entity());
        Assert.That(cell.Contents, Has.Count.EqualTo(2));
    }

    [Test]
    public void EntityCanBeRemovedFromCell() {
        var entity = new Entity();
        cell.PutContents(entity);
        cell.RemoveEntity(entity);
        Assert.That(cell.Contents, Is.Empty);
    }

    [Test]
    public void RemovingAnEntityOnlyRemovesThatEntity() {
        var entity1 = new Entity();
        var entity2 = new Entity();
        cell.PutContents(entity1);
        cell.PutContents(entity2);
        
        cell.RemoveEntity(entity1);
        Assert.That(cell.Contents, Has.Count.EqualTo(1));
        Assert.That(cell.Contents, Does.Contain(entity2));
    }

    [Test]
    public void EntityCannotBeAddedTwice() {
        var entity = new Entity();
        cell.PutContents(entity);
        cell.PutContents(entity);
        Assert.That(cell.Contents, Has.Count.EqualTo(1));
    }

    [Test]
    public void EmptyCellShouldBeTransparent() {
        Assert.That(cell.IsTransparent);
    }

    [Test]
    public void CellWithTransparentEntityShouldBeTransparent() {
        var entity = new Entity {BlocksSight = false};
        cell.PutContents(entity);
        Assert.That(cell.IsTransparent);
    }

    [Test]
    public void CellWithOpaqueEntityShouldNotBeTransparent() {
        var entity = new Entity {BlocksSight = true};
        cell.PutContents(entity);
        Assert.That(!cell.IsTransparent);
    }

    [Test]
    public void CellWithOpaqueAndTransparentContentsShouldNotBeTransparent() {
        var entity1 = new Entity {BlocksSight = false};
        cell.PutContents(entity1);
        var entity2 = new Entity {BlocksSight = true};
        cell.PutContents(entity2);
        Assert.That(!cell.IsTransparent);
    }

    [Test]
    public void EmptyCellShouldByPassable() {
        Assert.That(cell.IsPassable);
    }

    [Test]
    public void CellWithOnlyPassableEntityShouldBePassable() {
        var entity = new Entity();
        entity.AddComponent(new Movement() {BlocksMove = false});
        cell.PutContents(entity);
        Assert.That(cell.IsPassable);
    }

    [Test]
    public void CellWithOnlyBlockingEntityShouldNotBePassable() {
        var entity = new Entity();
        entity.AddComponent(new Movement() {BlocksMove = true});
        cell.PutContents(entity);
        Assert.That(!cell.IsPassable);
    }

    [Test]
    public void CellWithBlockingAndPassableContentsShouldNotBePassable() {
        var entity1 = new Entity();
        entity1.AddComponent(new Movement() {BlocksMove = false});
        cell.PutContents(entity1);

        var entity2 = new Entity();
        entity2.AddComponent(new Movement() {BlocksMove = true});
        cell.PutContents(entity2);
        Assert.That(!cell.IsPassable);
    }
}