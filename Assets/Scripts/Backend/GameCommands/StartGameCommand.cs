public struct StartGameCommand : IGameCommand {
    public GameCommandType CommandType => GameCommandType.StartGame;
}