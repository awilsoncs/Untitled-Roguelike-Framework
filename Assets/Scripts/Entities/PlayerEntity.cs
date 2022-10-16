public class PlayerEntity : Entity {
    public PlayerEntity(int id) : base(id) {}

    public override void GameUpdate() {
        switch (BoardController.Instance.GetUserInputAction()) {
            case "right":
                Move(1, 0);
                return;
            case "left":
                Move(-1, 0);
                return;
            case "up":
                Move(0, 1);
                return;
            case "down":
                Move(0, -1);
                return;
            case "spawn":
                var entity = BoardController.Instance.CreateEntityByIndex(1);
                entity.TeleportTo(X, Y);
                return;
        }
    }
}