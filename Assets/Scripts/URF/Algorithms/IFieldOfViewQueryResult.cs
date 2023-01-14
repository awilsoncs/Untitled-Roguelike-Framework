namespace URF.Algorithms {
  using URF.Common;

  /// <summary>
  /// Provide a queryable interface for field of view results.
  /// </summary>
  public interface IFieldOfViewQueryResult {

    /// <summary>
    /// Given a position, return whether that position is flagged as visible in the result.
    /// </summary>
    /// <param name="position">the position to check for visibility</param>
    /// <returns>true if the given position is visible, false otherwise</returns>
    bool IsVisible(Position position);

  }
}
