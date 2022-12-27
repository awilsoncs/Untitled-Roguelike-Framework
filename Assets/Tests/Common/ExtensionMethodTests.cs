namespace Tests.Common {
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common;

  public class ExtensionMethodTests {
    [Test]
    public void TryGetValueOrDefault_Should_ReturnExistingElement() {
      var testDict = new Dictionary<int, int> {
        { 0, 1 }
      };
      Assert.That(testDict.GetValueOrDefault(0, -1), Is.EqualTo(1));
    }

    [Test]
    public void TryGetValueOrDefault_Should_Return_DefaultElementIfQueryDoesntExist() {
      var testDict = new Dictionary<int, int> {
        { 0, 1 }
      };
      Assert.That(testDict.GetValueOrDefault(1, -1), Is.EqualTo(-1));
    }
  }
}
