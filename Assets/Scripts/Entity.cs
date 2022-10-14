public class Entity : IEntity {
    private IBoardController gc;
    private int id = 0;
    private int x, y = 0;

    public Entity(IBoardController gc, int id) {
        this.gc = gc;
        this.id = id;
    }

    public void Update() {
        switch (gc.GetUserInputAction()) {
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
                gc.SaveGame();
                return;
            case "load":
                gc.LoadGame();
                return;
            case "reload":
                gc.ReloadGame();
                return;
        }
    }

    public void Move(int dx, int dy) {
        x += dx;
        y += dy;
        gc.SetPawnPosition(id, x, y);
    }
}