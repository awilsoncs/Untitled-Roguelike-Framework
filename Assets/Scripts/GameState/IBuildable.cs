/// <summary>
/// Provide an interface for dungeon and/or worldbuilding systems to access the
/// GameState with more granularity than the GameClient.
/// </summary>
public interface IBuildable {
    void SetMainCharacter(int id);
    int CreateEntityAtPosition(string appearance, int x, int y);
    int MapWidth {get;}
    int MapHeight {get;}
}