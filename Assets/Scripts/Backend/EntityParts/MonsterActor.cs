using System;

public class MonsterActor : EntityPart {

    public override EntityPartType PartType => EntityPartType.MonsterActor;
    public override void GameUpdate()
    {
        // todo make this only happen if the entity can see the player
        var mainCharacter = GameState.GetMainCharacter();
        var mainCharacterPosition = (mainCharacter.X, mainCharacter.Y);
        var position = (Entity.X, Entity.Y);
        if (!GameState.FieldOfView.IsVisible(GameState, position, mainCharacterPosition)) {
            // entity can't see the player, just dawdle.
            return;
        }

        var costs = GetMovementCosts(GameState);
        // take a step along the path
        var path = GameState.Pathfinding.GetPath(costs, (Entity.X, Entity.Y), mainCharacterPosition);
        // todo if we're close just attack
        var nextStep = path[1];
        var mx = nextStep.Item1 - Entity.X;
        var my = nextStep.Item2 - Entity.Y;
        GameState.PushCommand(new MoveCommand(Entity.ID, mx, my));
    }

    private float[][] GetMovementCosts(IGameState gs)
    {
        (int width, int height) = gs.GetMapSize();
        float[][] costs = new float[width][];
        for (int x = 0; x < width; x++)
        {
            costs[x] = new float[height];
            for (int y = 0; y < height; y++)
            {
                costs[x][y] = gs.IsTraversable(x, y) ? 0.1f : 10000f;
            }
        }
        return costs;
    }

    public override void Recycle() {
        EntityPartPool<MonsterActor>.Reclaim(this);
    }
}