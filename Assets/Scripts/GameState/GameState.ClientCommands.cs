using System.Collections.Generic;

/// <summary>
/// Define commands available to the client.
/// </summary>
public partial class GameState : IGameState {
        public void MoveUp()
    {
        // todo actually do the move
        gameClient.EntityMoved(0, 0, 1);
    }

    public void MoveDown()
    {
        // todo actually do the move
        gameClient.EntityMoved(0, 0, -1);
    }

    public void MoveLeft()
    {
        // todo actually do the move
        gameClient.EntityMoved(0, -1, 0);
    }

    public void MoveRight()
    {
        // todo actually do the move
        gameClient.EntityMoved(0, 1, 0);
    }

    public void SpawnCrab()
    {
        CreateEntityAtLocation("crab", 0, 0);
        gameClient.EntityCreated(0, "crab", 0, 0);
    }

    public IEnumerable<IEntity> GetEntities()
    {
        return entities;
    }
}