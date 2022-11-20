namespace Tests.Server {
  using NUnit.Framework;
  using UnityEngine;
  using URF.Common.Entities;
  using URF.Common.Persistence;
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


    // mock component
    private sealed class ComponentA : BaseComponent {
      public IGameDataReader SavedReader;
      public IGameDataWriter SavedWriter;

      public override void Load(IGameDataReader reader) {
        this.SavedReader = reader;
      }

      public override void Save(IGameDataWriter writer) {
        this.SavedWriter = writer;
      }

      public override string EntityString => "testString";
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

    private sealed class MockGameDataWriter : IGameDataWriter {
      public void Write(float value) {
        // no op
      }

      public void Write(bool value) {
        // no op
      }

      public void Write(int value) {
        // no op
      }

      public void Write(string value) {
        // no op
      }

      public void Write(Vector3 value) {
        // no op
      }
    }

    private sealed class MockGameDataReader : IGameDataReader {
      public int Version => 0;

      public bool ReadBool() {
        return true;
      }

      public float ReadFloat() {
        return 0.0f;
      }

      public int ReadInt() {
        return 1;
      }

      public string ReadString() {
        return "test string";
      }

      public Vector3 ReadVector3() {
        return Vector3.zero;
      }
    }

    [Test]
    public void Entity_Should_AskComponentsToSave() {
      var component = new ComponentA();

      this.testEntity.AddComponent(component);
      var mockWriter = new MockGameDataWriter();

      this.testEntity.Save(mockWriter);
      Assert.That(component.SavedWriter, Is.EqualTo(mockWriter));
    }

    [Test]
    public void Entity_Should_AskComponentsToLoad() {
      var component = new ComponentA();

      this.testEntity.AddComponent(component);
      var mockReader = new MockGameDataReader();

      this.testEntity.Load(mockReader);
      Assert.That(component.SavedReader, Is.EqualTo(mockReader));
    }

    [Test]
    public void Entity_Should_QueryComponentsForToStringDetails() {
      var component = new ComponentA();
      this.testEntity.ID = 1;

      this.testEntity.AddComponent(component);
      string entityRepr = this.testEntity.ToString();
      Assert.That(entityRepr, Is.EqualTo("1::testString"));
    }

    [Test]
    public void Entity_Should_NotCrashWithNullWriter() {
      this.testEntity.Save(null);
    }

    [Test]
    public void Entity_Should_NotCrashWithNullReader() {
      this.testEntity.Load(null);
    }

  }
}
