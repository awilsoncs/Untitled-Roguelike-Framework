using System.Collections.Generic;
using URFCommon;

public class FieldOfViewQueryResult : IFieldOfViewQueryResult {
    private readonly Dictionary<Position, bool> results;
    public FieldOfViewQueryResult(in Dictionary<Position, bool> results) {
        this.results = results;
    }
    public bool IsVisible(Position p) {
        return results.GetValueOrDefault<Position, bool>(p);
    }
}
