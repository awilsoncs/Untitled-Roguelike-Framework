public static class DungeonBuilder {
    public static void Build(IGameState gs) {
        for (int i = 0; i < gs.MapWidth; i++) {
            gs.CreateEntityAtLocation("wall", i, 0);
            gs.CreateEntityAtLocation("wall", i, gs.MapHeight - 1);
        }
        for (int i = 1; i < gs.MapHeight - 1; i++) {
            gs.CreateEntityAtLocation("wall", 0, i);
            gs.CreateEntityAtLocation("wall", gs.MapWidth - 1, i);
        }
        gs.CreateEntityAtLocation("player", gs.MapWidth / 2, gs.MapHeight / 2);
    }
}