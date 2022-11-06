using URFCommon;

public interface IFieldOfView {
    IFieldOfViewQueryResult CalculateFOV(IGameState gameState, Position pos);
    bool IsVisible(IGameState gameState, Position start, Position end);
}