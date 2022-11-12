using System.Collections.Generic;
using URF.Common;

namespace URF.Server.FieldOfView {
  public class FieldOfViewQueryResult : IFieldOfViewQueryResult {

    private readonly Dictionary<Position, bool> _results;

    public FieldOfViewQueryResult(in Dictionary<Position, bool> results) {
      _results = results;
    }

    public bool IsVisible(Position p) {
      return _results.GetValueOrDefault(p);
    }

  }
}
