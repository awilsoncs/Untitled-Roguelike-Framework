namespace Tests.Common {
  using NUnit.Framework;
  using URF.Common;

  public class PositionTests {
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
      Position a = new(5, 10);
      Position b = new(5, 10);
      Assert.That(a, Is.EqualTo(b));
    }

    [Test]
    public void TestToString() {
      Position a = new(8, 13);
      Position b = new(5, 8);

      Assert.That(a.ToString(), Is.EqualTo("(8,13)"));
      Assert.That(b.ToString(), Is.EqualTo("(5,8)"));
    }

    [Test]
    public void TestAddition() {
      Position a = new(8, 13);
      Position b = new(5, 8);

      Assert.That(a + b, Is.EqualTo(new Position(13, 21)));
    }

  }
}
