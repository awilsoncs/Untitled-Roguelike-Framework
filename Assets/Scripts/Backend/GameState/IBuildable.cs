using System;
using URFCommon;

public interface IBuildable {
    void SetMainCharacter(int id);
    int CreateEntityAtPosition(string appearance, Position position);
    Cell GetCell(Position position);
    int MapWidth {get;}
    int MapHeight {get;}
}