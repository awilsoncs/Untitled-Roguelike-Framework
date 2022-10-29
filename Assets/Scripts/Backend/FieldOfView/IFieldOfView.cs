public interface IFieldOfView {
    IFieldOfViewQueryResult CalculateFOV(IGameState gameState, int x0, int y0);
    bool IsVisible(IGameState gameState, (int, int) start, (int, int) end);
}