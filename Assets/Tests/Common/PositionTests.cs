using NUnit.Framework;
using URF.Common;

namespace Tests.Common
{
    public class PositionTests
    {
        [Test]
        public void TestImplicitConversionOperator() {
            Position pos = (13, 17);
            Assert.That(pos.X, Is.EqualTo(13));
            Assert.That(pos.Y, Is.EqualTo(17));
        }

        [Test]
        public void TestDeconstructor() {
            (int x, int y) = new Position(5, 8);
            Assert.That(x, Is.EqualTo(5));
            Assert.That(y, Is.EqualTo(8));
        }

        [Test]
        public void TestEquality() {
            var a = new Position(5, 10);
            var b = new Position(5, 10);
            Assert.That(a, Is.EqualTo(b));
        }
    }
}