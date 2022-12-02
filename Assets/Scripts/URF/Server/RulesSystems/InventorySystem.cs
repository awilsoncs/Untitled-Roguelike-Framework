namespace URF.Server.RulesSystems {
  using URF.Common.GameEvents;

  /// <summary>
  /// Defines inventory handling for entities.
  /// </summary>
  public class InventorySystem : BaseRulesSystem {
    public override void HandleGetAction(GetAction getAction) {
      this.GameState.Delete(getAction.Target);
    }
  }

}
