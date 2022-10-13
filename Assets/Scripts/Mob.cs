public class Mob : IUpdateable {
    private IGameController gc;
    private int id = 0;
    private int x, y = 0;

    public Mob(IGameController gc, int id) {
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
        }
    }

    public void Move(int dx, int dy) {
        x += dx;
        y += dy;
        gc.SetPawnPosition(id, x, y);
    }
}