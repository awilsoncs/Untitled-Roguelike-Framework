namespace Tests.Server {
  using NUnit.Framework;
  using URF.Common;
  using URF.Server.FieldOfView;

  [TestFixture]
  public class RaycastingFovTests {
    private readonly RaycastingFov fov = new();

    private void RunLosTestCase(bool[,] transparency, Position start, Position end, bool expected) {
      // the FOV system provides two ways of determining visibility- a batch FOV system and a single
      // point LOS system. The results should always be the same.
      IFieldOfViewQueryResult fovResult = this.fov.CalculateFOV(transparency, start);
      Assert.That(fovResult.IsVisible(end), Is.EqualTo(expected));

      bool losResult = this.fov.IsVisible(transparency, start, end);
      Assert.That(losResult, Is.EqualTo(expected));
    }

    [Test]
    public void RaycastingFov_Should_SeeSameTile() {
      this.RunLosTestCase(
        new bool[1, 1] { { true } },
        (0, 0), (0, 0),
        true
      );
    }

    [Test]
    public void RaycastingFov_Should_SeeNeighbor() {
      this.RunLosTestCase(
        new bool[1, 2] { { true, true } },
        (0, 0), (0, 1),
        true
      );
    }

    [Test]
    public void RaycastingFov_Should_SeeBlockingNeighbor() {
      this.RunLosTestCase(
        new bool[1, 2] { { true, false } },
        (0, 0), (0, 1),
        true
      );
    }


    [Test]
    public void RaycastingFov_ShouldNot_SeeThroughBlockingNeighbor() {
      this.RunLosTestCase(
        new bool[1, 3] { { true, false, true } },
        (0, 0), (0, 2),
        false
      );
    }

    [Test]
    public void RayCastingFov_Should_SeeAroundSimpleCorner() {
      this.RunLosTestCase(
        new bool[2, 2] {
          { true, false },
          { true, true }
        },
        (0, 0), (1, 1),
        true
      );
    }

    [Test]
    public void RayCastingFov_Should_SeeThroughPinchCorner() {
      // debatable whether this is correct functionality, but it's how it works for now so I'll
      // keep it. -AW
      this.RunLosTestCase(
        new bool[2, 2] {
          { true, false },
          { false, true }
        },
        (0, 0), (1, 1),
        true
      );
    }
  }
}
