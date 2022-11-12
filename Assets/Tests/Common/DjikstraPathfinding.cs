using System.Collections.Generic;
using NUnit.Framework;
using URF.Common;
using URF.Server.Pathfinding;

namespace Tests.Common
{
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
            Assert.That(path.Contains((0, 0)), "Path should include the start node.");
            Assert.That(path.Count, Is.EqualTo(1), "Path should only include the start node.");
        }

        [Test]
        public void DjikstraPathfinderSolvesTrivialCase() {
            float[][] costs = {
                new float[] {0.1f, 0.1f},
                new float[] {0.1f, 0.1f} 
            };
            var path = pf.GetPath(costs, (0, 0), (1, 1));

            Assert.That(path.Count, Is.EqualTo(3), "Path should be three steps.");
            Assert.That(path[0], Is.EqualTo(new Position(0, 0)), "Path should include the start.");
            Assert.That(path[2], Is.EqualTo(new Position(1, 1)), "Path should end at the end.");
        }

        [Test]
        public void DjikstraPathfinderTakesEasyWindingRoute() {
            // In this test, we test that the algorithm follows the free
            // but longer route.
            float[][] costs = {
                new float[] {0f, 0f, 0f, 0f, 0f},
                new float[] {0f, 1f, 1f, 1f, 1f},
                new float[] {0f, 0f, 0f, 0f, 0f},
                new float[] {1f, 1f, 1f, 1f, 0f},
                new float[] {0f, 0f, 0f, 0f, 0f}
            };
            var expectedPath = new List<(int, int)> {
                (4, 0), (4, 1), (4, 2), (4, 3), (4, 4),
                (3, 4),
                (2, 4), (2, 3), (2, 2), (2, 1), (2, 0),
                (1, 0),
                (0, 0), (0, 1), (0, 2), (0, 3), (0, 4)
            };
            var actualPath = pf.GetPath(costs, (4, 0), (0, 4));
            for (int i = 0; i < expectedPath.Count; i++) {
                Assert.That(
                    (Position)expectedPath[i],
                    Is.EqualTo(actualPath[i]),
                    "Pathfinder should stay on the expected path."
                );
            }
        }
    }
}