public class PlayerBrain : EntityPart {

    public override EntityPartType PartType => EntityPartType.PlayerBrain; 

    public override void GameUpdate() {
        switch (BoardController.Instance.GetUserInputAction()) {
            case "right":
                Entity.Move(1, 0);
                return;
            case "left": 
                Entity.Move(-1, 0);
                return;
            case "up":
                Entity.Move(0, 1);
                return;
            case "down":
                Entity.Move(0, -1);
                return;
            case "spawn":
                var entity = BoardController.Instance.CreateEntityAtLocation(
                    "crab", Entity.X+1, Entity.Y+1);
                return;
        }
    }
    public override void Recycle()
    {
        // todo detail entity death cycle
        EntityPartPool<PlayerBrain>.Reclaim(this);
    }

    public override void Save(GameDataWriter writer) {}
    public override void Load(GameDataReader reader) {}
}