public class Entity : PersistableObject{
    private int id = 0;
    private int x, y = 0;

    public Entity(int id) {
        this.id = id;
    }

    public void GameUpdate() {
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
            case "save":
                BoardController.Instance.SaveGame();
                return;
            case "load":
                BoardController.Instance.LoadGame();
                return;
            case "reload":
                BoardController.Instance.ReloadGame();
                return;
        }
    }

    public void Move(int dx, int dy) {
        x += dx;
        y += dy;
        BoardController.Instance.SetPawnPosition(id, x, y);
    }

    public override void Save(GameDataWriter writer) {
    }

    public override void Load(GameDataReader reader) {
    }
}