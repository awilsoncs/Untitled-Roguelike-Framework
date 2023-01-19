namespace Tests.Server {
  using NUnit.Framework;
  using URF.Common;
  using URF.Algorithms;

  [TestFixture]
  public class RaycastingFovTests {
    private readonly RaycastingFov fov = new();

    private void RunLosTestCase(bool[,] transparency, Position start, Position end, bool expected) {
      // the FOV system provides two ways of determining visibility- a batch FOV system and a single
      // point LOS system. The results should always be the same.
      IFieldOfViewQueryResult fovResult = this.fov.CalculateFov(transparency, start);
      Assert.That(fovResult.IsVisible(end), Is.EqualTo(expected));

      bool losResult = this.fov.IsVisible(transparency, start, end);
      Assert.That(losResult, Is.EqualTo(expected));
    }

    [Test]
    public void RaycastingFov_Should_SeeSameTile() {
      this.RunLosTestCase(
        new bool[,] { { true } },
        (0, 0), (0, 0),
        true
      );
    }

    [Test]
    public void RaycastingFov_Should_SeeNeighbor() {
      this.RunLosTestCase(
        new bool[,] { { true, true } },
        (0, 0), (0, 1),
        true
      );
    }

    [Test]
    public void RaycastingFov_Should_SeeBlockingNeighbor() {
      this.RunLosTestCase(
        new bool[,] { { true, false } },
        (0, 0), (0, 1),
        true
      );
    }


    [Test]
    public void RaycastingFov_ShouldNot_SeeThroughBlockingNeighbor() {
      Position start = (0, 0);
      Position end = (0, 2);
      this.RunLosTestCase(
        new bool[,] { { true, false, true } },
        start, end,
        false
      );
    }

    [Test]
    public void RayCastingFov_Should_SeeAroundSimpleCorner() {
      this.RunLosTestCase(
        new bool[,] {
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
        new bool[,] {
          { true, false },
          { false, true }
        },
        (0, 0), (1, 1),
        true
      );
    }
  }
}
