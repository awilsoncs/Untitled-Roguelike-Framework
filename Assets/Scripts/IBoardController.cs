public interface IBoardController {
    int MapWidth { get; set; }
    int MapHeight { get; set; }

    // todo this probably doesn't belong here
    /// <summary>
    /// Create an entity at a given location using the factory blueprint name.
    /// </summary>
    /// <param name="blueprintName">The name of the Entity type in the factory</param>
    /// <param name="x">The horizontal coordinate at which to create the Entity</param>
    /// <param name="y">The vertical coordinate at which to create the Entity</param>
    /// <returns>A reference to the created Entity</returns>
    Entity CreateEntityAtLocation(System.String blueprintName, int x, int y);

    /// <summary>
    /// Return whether this tile can legally be stepped into.
    /// </summary>
    /// <param name="x">The horizontal coordinate to check</param>
    /// <param name="y">The vertical position to check</param>
    /// <returns>True if the position is legal to step to, False otherwise</returns>
    bool IsLegalMove(int x, int y);

    /// <summary>
    /// Places an Entity on the board.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x">Horizontal destination coordinate of the Entity</param>
    /// <param name="y">Vertical destination coordinate of the Entity</param>
    void PlacePawn(int id, int x, int y);

    /// <summary>
    /// Moves an Entity from one place on the Board to another.
    /// </summary>
    /// <param name="id">ID of the entity to move</param>
    /// <param name="x0">Original Horizontal coordinate of the Entity</param>
    /// <param name="y0">Original Vertical coordinate of the Entity</param>
    /// <param name="x1">Horizontal destination coordinate of the Entity</param>
    /// <param name="y1">Vertical destination coordinate of the Entity</param>
    void MovePawn(int id, int x0, int y0, int x1, int y1);

    // todo this doesn't belong here
    /// <summary>
    /// Return a string of the user action. Some actions are intercepted by the
    ///  BoardController and instead return none.
    /// </summary>
    /// <returns>A string descriptor of the user's action.</returns>
    System.String GetUserInputAction(); 

    /// <summary>
    /// Call to schedule the FOV to be recalculated at the end of this game
    /// loop.
    /// </summary>
    void  RecalculateFOV();
}