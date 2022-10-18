public static class DungeonBuilder {
    public static void Build(BoardController bc) {
        for (int i = 0; i < bc.MapWidth; i++) {
            bc.CreateEntityAtLocation("wall", i, 0);
            bc.CreateEntityAtLocation("wall", i, bc.MapHeight - 1);
        }
        for (int i = 1; i < bc.MapHeight - 1; i++) {
            bc.CreateEntityAtLocation("wall", 0, i);
            bc.CreateEntityAtLocation("wall", bc.MapWidth - 1, i);
        }
        bc.CreateEntityAtLocation("player", bc.MapWidth / 2, bc.MapHeight / 2);
    }
}