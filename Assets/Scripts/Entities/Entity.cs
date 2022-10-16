public class Entity : PersistableObject{
    public int EntityType {get; set;}
    public int ID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Entity(int id) {
        ID = id;
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
        BoardController.Instance.SetPawnPosition(ID, X, Y);
    }

    public override void Save(GameDataWriter writer) {
        base.Save(writer);
        writer.Write(X);
        writer.Write(Y);
    }

    public override void Load(GameDataReader reader) {
        base.Load(reader);
        X = reader.ReadInt();
        Y = reader.ReadInt();
    }

    public void Recycle() {
        BoardController.Instance.entityFactory.Reclaim(this);
    }

}
