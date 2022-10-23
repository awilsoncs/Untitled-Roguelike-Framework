using System.Collections.Generic;
public class FieldOfViewQueryResult : IFieldOfViewQueryResult {
    private Dictionary<(int, int), bool> results;
    public FieldOfViewQueryResult(in Dictionary<(int, int), bool> results) {
        this.results = results;
    }
    public bool IsVisible(int x, int y) {
        return results.GetValueOrDefault<(int, int), bool>((x, y));
    }
}
