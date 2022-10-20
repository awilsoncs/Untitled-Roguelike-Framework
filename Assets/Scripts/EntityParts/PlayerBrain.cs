public class PlayerBrain : EntityPart {

    public override EntityPartType PartType => EntityPartType.PlayerBrain; 

    public override void GameUpdate(IBoardController bc) {
        var x = Entity.X;
        var y = Entity.Y;
        switch (bc.GetUserInputAction()) {
            case "right":
                bc.MovePawn(Entity.ID, x, y, x+1, y);
                return;
            case "left": 
                bc.MovePawn(Entity.ID, x, y, x-1, y);
                return;
            case "up":
                bc.MovePawn(Entity.ID, x, y, x, y+1);
                return;
            case "down": 
                bc.MovePawn(Entity.ID, x, y, x, y-1);
                return;
            case "spawn":
                var entity = bc.CreateEntityAtLocation("crab", x+1, y+1);
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