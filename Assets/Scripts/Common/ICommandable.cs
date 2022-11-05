
namespace URFCommon {
    /// <summary>
    /// Push a command to this object.
    /// </summary>
    public interface ICommandable {
        void PushCommand(IGameCommand cm);
    }
}