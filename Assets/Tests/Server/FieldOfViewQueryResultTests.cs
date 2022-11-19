namespace Tests.Server {
  using System.Collections.Generic;
  using NUnit.Framework;
  using URF.Common;
  using URF.Server.FieldOfView;

  public class FieldOfViewQueryResultTests {
    private readonly Position cell = (5, 8);

    [Test]
    public void FieldOfViewQueryResult_Should_ShowVisiblePositionIsVisible() {
      var results = new Dictionary<Position, bool> {
        [this.cell] = true
      };

      var query = new FieldOfViewQueryResult(results);
      Assert.That(query.IsVisible((5, 8)), "Visible position wasn't returned visible.");
    }

    [Test]
    public void FieldOfViewQueryResult_Should_ShowNotVisiblePositionIsNotVisible() {
      var results = new Dictionary<Position, bool> {
        [(5, 8)] = false
      };

      var query = new FieldOfViewQueryResult(results);
      Assert.That(!query.IsVisible((5, 8)), "Not visible position was returned visible.");
    }

    [Test]
    public void FieldOfViewQueryResult_Should_DefaultToNotVisible() {
      var results = new Dictionary<Position, bool> {
        [(5, 8)] = false
      };

      var query = new FieldOfViewQueryResult(results);
      Assert.That(!query.IsVisible((8, 13)), "Query result should default to not visible.");
    }

    [Test]
    public void FieldOfViewQueryResult_Should_OnlyShowVisibleIfThisExactCellIsVisible() {
      // test that results don't return true if column/row are right but both aren't
      var results = new Dictionary<Position, bool> {
        [(5, 8)] = true,
        [(6, 9)] = true
      };

      var query = new FieldOfViewQueryResult(results);
      Assert.That(
        !query.IsVisible((5, 9)),
        "Query result should require row/column pair to be visible."
      );
    }
  }
}
