public class Entity : PersistableObject{
    private int id = 0;
    public int EntityType {get; set;}

    public int ID {
        get {
            return id;
        }
        set {
            this.id = value;
        }
    }

    public int X { get; set; }
    public int Y { get; set; }

    public Entity(int id) {
        this.id = id;
    }

    public virtual void GameUpdate() {
        // override
    }

    public void Move(int dx, int dy) {
        TeleportTo(X+dx, Y+dy);
    }

    public void TeleportTo(int x, int y) {
        X = x;
        Y = y;
        BoardController.Instance.SetPawnPosition(id, X, Y);
    }

    public override void Save(GameDataWriter writer) {
        writer.Write(ID);
        writer.Write(X);
        writer.Write(Y);
    }

    public override void Load(GameDataReader reader) {
        ID = reader.ReadInt();
        X = reader.ReadInt();
        Y = reader.ReadInt();
    }
}
