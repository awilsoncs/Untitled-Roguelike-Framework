using NUnit.Framework;

public class DjikstraPathfindingTests
{
    DjikstraPathfinding pf;

    [SetUp]
    public void Setup() {
        pf = new DjikstraPathfinding();
    }

    [Test]
    public void DjikstraPathfinderSolvesNullCase() {
        float[][] costs = {
            new float[] {0f, 0f},
            new float[] {0f, 0f} 
        };
        var path = pf.GetPath(costs, (0, 0), (0, 0));

        Assert.That(path, Is.Empty);
    }

    [Test]
    public void DjikstraPathfinderSolvesTrivialCase() {
        float[][] costs = {
            new float[] {0.1f, 0.1f},
            new float[] {0.1f, 0.1f} 
        };
        var path = pf.GetPath(costs, (0, 0), (1, 1));

        Assert.That(path.Count, Is.EqualTo(2), "Path should be two steps.");
        Assert.That(path[0], Is.Not.EqualTo((0, 0)), "Path should not include the start.");
        Assert.That(path[0], Is.Not.EqualTo((1, 1)), "Path should not include the start.");
        Assert.That(path[1], Is.EqualTo((1, 1)), "Path should end at the end.");
    }
}