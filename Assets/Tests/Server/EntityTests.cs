namespace Tests.Server {
  using NUnit.Framework;
  using URF.Common.Entities;
  using URF.Server;

  public class EntityTests {
    private Entity testEntity;

    [SetUp]
    public void SetUp() {
      this.testEntity = new Entity();
    }

    [Test]
    public void Entity_Should_HaveGivenId() {
      const int givenId = 57;
      this.testEntity.ID = givenId;
      Assert.That(this.testEntity.ID, Is.EqualTo(givenId));
    }

    [Test]
    public void Entity_Should_HaveGivenTransparency() {
      this.testEntity.BlocksSight = true;
      Assert.That(this.testEntity.BlocksSight);

      this.testEntity.BlocksSight = false;
      Assert.That(!this.testEntity.BlocksSight);
    }

    [Test]
    public void Entity_Should_HaveGivenVisibility() {
      this.testEntity.IsVisible = true;
      Assert.That(this.testEntity.IsVisible);

      this.testEntity.IsVisible = false;
      Assert.That(!this.testEntity.IsVisible);
    }


    private sealed class ComponentA : BaseComponent {
      // stub test class
    }


    private sealed class ComponentB : BaseComponent {
      // stub test class
    }

    [Test]
    public void Entity_Should_HaveGivenComponent() {
      var component = new ComponentA();

      this.testEntity.AddComponent(component);
      ComponentA foundComponent = this.testEntity.GetComponent<ComponentA>();
      Assert.That(foundComponent, Is.EqualTo(component));
    }

    [Test]
    public void Entity_ShouldNot_HaveNotGivenComponent() {
      var component = new ComponentA();

      this.testEntity.AddComponent(component);
      ComponentB foundComponent = this.testEntity.GetComponent<ComponentB>();
      Assert.That(foundComponent, Is.Null);
    }


  }
}
