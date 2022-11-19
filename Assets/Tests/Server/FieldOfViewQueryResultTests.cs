namespace Tests.Server {
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common;
  using URF.Server.FieldOfView;

  public class FieldOfViewQueryResultTests {
    private readonly Position testCell = (5, 8);

    [Test]
    public void FieldOfViewQueryResult_Should_ShowVisiblePositionIsVisible() {
      var results = new Dictionary<Position, bool> {
        [this.testCell] = true
      };

      var query = new FieldOfViewQueryResult(results);
      Assert.That(query.IsVisible(this.testCell), "Visible position wasn't returned visible.");
    }

    [Test]
    public void FieldOfViewQueryResult_Should_ShowNotVisiblePositionIsNotVisible() {
      var results = new Dictionary<Position, bool> {
        [this.testCell] = false
      };

      var query = new FieldOfViewQueryResult(results);
      Assert.That(!query.IsVisible(this.testCell), "Not visible position was returned visible.");
    }

    [Test]
    public void FieldOfViewQueryResult_Should_DefaultToNotVisible() {
      var results = new Dictionary<Position, bool> {
        [this.testCell] = false
      };

      var query = new FieldOfViewQueryResult(results);
      var otherPosition = new Position(8, 13);
      Assert.That(!query.IsVisible(otherPosition), "Query result should default to not visible.");
    }

    [Test]
    public void FieldOfViewQueryResult_Should_OnlyShowVisibleIfThisExactCellIsVisible() {
      // test that results don't return true if column/row are right but both aren't
      var otherVisibleCell = new Position(6, 9);
      var results = new Dictionary<Position, bool> {
        [this.testCell] = true,
        [otherVisibleCell] = true
      };

      var notVisibleCell = new Position(5, 9);

      var query = new FieldOfViewQueryResult(results);
      Assert.That(
        !query.IsVisible(notVisibleCell),
        "Query result should require row/column pair to be visible."
      );
    }
  }
}
