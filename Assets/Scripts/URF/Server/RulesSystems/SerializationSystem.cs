using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class SerializationSystem : BaseRulesSystem {

    private const int saveVersion = 1;

    private PersistentStorage _persistentStorage;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _persistentStorage = pluginBundle.PersistentStorage;
    }

    [ActionHandler(GameEventType.Save)]
    public void HandleSaveAction(IGameState gs, IActionEventArgs cm) {
      _persistentStorage.Save(gs, saveVersion);
    }

    [ActionHandler(GameEventType.Load)]
    public void HandleLoadAction(IGameState gs, IActionEventArgs cm) {
      _persistentStorage.Load(gs);
    }
  }
}
