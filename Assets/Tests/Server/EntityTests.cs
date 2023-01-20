namespace Tests.Server {
  using NUnit.Framework;
  using UnityEngine;
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
    public void Entity_Should_NotCrashWithNullWriter() {
      this.testEntity.Save(null);
    }

    [Test]
    public void Entity_Should_NotCrashWithNullReader() {
      this.testEntity.Load(null);
    }

  }
}
