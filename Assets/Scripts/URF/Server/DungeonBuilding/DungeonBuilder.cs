using URF.Server.GameState;
using URF.Server.RandomGeneration;

namespace URF.Server.DungeonBuilding {
  public static class DungeonBuilder {

    public static void Build(IBuildable gs, IRandomGenerator rng) {
      for(int i = 0; i < gs.MapWidth; i++) {
        gs.CreateEntityAtPosition("wall", (i, 0));
        gs.CreateEntityAtPosition("wall", (i, gs.MapHeight - 1));
      }

      for(int i = 1; i < gs.MapHeight - 1; i++) {
        gs.CreateEntityAtPosition("wall", (0, i));
        gs.CreateEntityAtPosition("wall", (gs.MapWidth - 1, i));
      }

      int mainCharId = gs.CreateEntityAtPosition("player", (gs.MapWidth / 2, gs.MapHeight / 2));
      gs.SetMainCharacter(mainCharId);

      for(int i = 0; i < 4; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        gs.CreateEntityAtPosition("crab", (x, y));
      }

      for(int i = 0; i < 4; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        gs.CreateEntityAtPosition("wall", (x, y));
      }

      for(int i = 0; i < 2; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        gs.CreateEntityAtPosition("healthPotion", (x, y));
      }
    }

  }
}
