using System.Collections.Generic;

/// <summary>
/// Define commands available to the client.
/// </summary>
public partial class GameState : IGameState {
        public void MoveUp()
    {
        gameClient.EntityMoved(0, 0, 1);
    }

    public void MoveDown()
    {
        gameClient.EntityMoved(0, 0, -1);
    }

    public void MoveLeft()
    {
        gameClient.EntityMoved(0, -1, 0);
    }

    public void MoveRight()
    {
        gameClient.EntityMoved(0, 1, 0);
    }

    public void SpawnCrab()
    {
        gameClient.EntityCreated(0, "crab", 0, 0);
    }

    public IEnumerable<IEntity> GetEntities()
    {
        return entities;
    }
}