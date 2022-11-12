
namespace URF.Common.GameEvents {
    public struct DebugCommand : IGameEvent {
        public enum DebugMethod {
            SpawnCrab
        }

        public bool IsCommand => true;

        public GameEventType EventType => GameEventType.DebugCommand;

        public DebugMethod Method {get;}

        public DebugCommand(DebugMethod method) {
            Method = method;
        }

        public static DebugCommand SpawnCrab() {
            return new DebugCommand(DebugMethod.SpawnCrab);
        }
    }
}
