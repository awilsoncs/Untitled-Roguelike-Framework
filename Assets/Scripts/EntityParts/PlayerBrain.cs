public class PlayerBrain : EntityPart {
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
                var entity = BoardController.Instance.CreateEntityByName("crab");
                entity.TeleportTo(Entity.X, Entity.Y);
                return;
        }
    }
    public override void Recycle()
    {
        EntityPartPool<PlayerBrain>.Reclaim(this);
    }

    public override void Save(GameDataWriter writer) {}
    public override void Load(GameDataReader reader) {}
}