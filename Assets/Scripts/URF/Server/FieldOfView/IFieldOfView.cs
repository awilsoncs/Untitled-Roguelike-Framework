using URF.Common;
using URF.Server.GameState;

namespace URF.Server.FieldOfView
{
    public interface IFieldOfView {
        IFieldOfViewQueryResult CalculateFOV(IGameState gameState, Position pos);
        bool IsVisible(IGameState gameState, Position start, Position end);
    }
}