using URF.Common;

namespace URF.Server.GameState {
  public interface IBuildable {

    void SetMainCharacter(int id);

    int CreateEntityAtPosition(string blueprintName, Position position);

    Cell GetCell(Position position);

    int MapWidth { get; }

    int MapHeight { get; }

  }
}
