using System.Collections.Generic;

/// <summary>
/// Define commands available to the client.
/// </summary>
public partial class GameState : IGameState {
    public void MoveUp()
    {
        MoveEntity(mainCharacter.ID, mainCharacter.X, mainCharacter.Y+1);
    }

    public void MoveDown()
    {
        MoveEntity(mainCharacter.ID, mainCharacter.X, mainCharacter.Y-1);
    }

    public void MoveLeft()
    {
        MoveEntity(mainCharacter.ID, mainCharacter.X-1, mainCharacter.Y);
    }

    public void MoveRight()
    {
        MoveEntity(mainCharacter.ID, mainCharacter.X+1, mainCharacter.Y);
    }

    public void SpawnCrab()
    {
        CreateEntityAtLocation("crab", 0, 0);
        gameClient.PostEvent(new EntityCreatedEvent(0, "crab", 0, 0));
    }

    public IEnumerable<IEntity> GetEntities()
    {
        return entities;
    }
}