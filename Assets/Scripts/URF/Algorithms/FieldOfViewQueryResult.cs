namespace URF.Algorithms {
  using System.Collections.Generic;
  using URF.Common;

  public sealed class FieldOfViewQueryResult : IFieldOfViewQueryResult {

    private readonly Dictionary<Position, bool> results;

    public FieldOfViewQueryResult(in Dictionary<Position, bool> results) {
      this.results = results;
    }

    public bool IsVisible(Position p) {
      return this.results.GetValueOrDefault(p);
    }
  }
}
