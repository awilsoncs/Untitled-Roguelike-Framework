namespace URF.Common.GameState {
  using System.Collections.Generic;
  using URF.Common.Entities;

  public interface IReadOnlyGameState<out TCell> where TCell : IReadOnlyCell {
    /// <summary>
    /// Get a Position representing the dimensions of the map
    /// </summary>
    /// <value>A Position representing the int x int dimensions of the map.</value>
    Position MapSize {
      get;
    }

    /// <summary>
    /// Get a collection of all Entities in the game.
    /// </summary>
    /// <returns>
    /// An IReadOnlyCollection<IEntity> containing all entities in the game state.
    /// </returns>
    IReadOnlyCollection<IEntity> GetAllEntities();

    /// <summary>
    /// Get a single map cell.
    /// </summary>
    /// <param name="position">The position of the cell to get.</param>
    /// <returns>A Cell object</returns>
    TCell GetCell(Position position);

    Position LocateEntityOnMap(IEntity entity);
  }
}
