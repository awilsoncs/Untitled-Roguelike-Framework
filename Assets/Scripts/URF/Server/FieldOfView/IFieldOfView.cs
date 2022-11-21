namespace URF.Server.FieldOfView {
  using URF.Common;

  /// <summary>
  /// Interface for any system that handles details of visibility between two objects. The FOV
  /// receives a 2D matrix of booleans where true indicates transparent and false indicates opaque.
  /// </summary>
  public interface IFieldOfView {

    /// <summary>
    /// Calculate a field of view from a given point.
    /// </summary>
    /// <param name="transparency">a 2D map of transparency where true equals transparent
    /// and false equals opaque.</param>
    /// <param name="position">the position to calculate field of view from</param>
    /// <returns></returns>
    IFieldOfViewQueryResult CalculateFOV(bool[,] costs, Position position);

    /// <summary>
    /// Calculate whether an end point is visible to a start point, i.e. check line of sight.
    /// </summary>
    /// <param name="transparency">a 2D map of transparency where true equals transparent
    /// and false equals opaque.</param>
    /// <param name="start">the position of the viewer</param>
    /// <param name="end">the position to check for visibility</param>
    /// <returns></returns>
    bool IsVisible(bool[,] transparency, Position start, Position end);

  }
}
