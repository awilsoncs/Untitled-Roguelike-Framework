using URF.Common;

namespace URF.Server.FieldOfView
{
    public interface IFieldOfViewQueryResult {
        bool IsVisible(Position p);
    }
}