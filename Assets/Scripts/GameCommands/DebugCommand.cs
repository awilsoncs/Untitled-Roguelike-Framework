public class DebugCommand : IGameCommand {
    public enum DebugMethod {
        SpawnCrab,
        RevealMap
    }

    public GameCommandType CommandType => GameCommandType.Debug;

    public DebugMethod Method {get;}

    public DebugCommand(DebugMethod method) {
        Method = method;
    }

    public static DebugCommand SpawnCrab() {
        return new DebugCommand(DebugMethod.SpawnCrab);
    }

    public static DebugCommand RevealMap() {
        return new DebugCommand(DebugMethod.RevealMap);
    }
}