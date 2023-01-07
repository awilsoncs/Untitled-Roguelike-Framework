namespace URF.Server.RulesSystems {
  using URF.Common.GameEvents;

  /// <summary>
  /// Defines inventory handling for entities.
  /// </summary>
  public class InventorySystem : BaseRulesSystem {
    public override void HandleInventoryEvent(InventoryEvent inventoryEvent) {
      if (inventoryEvent.Action != InventoryEvent.InventoryAction.WantsToGet) {
        return;
      }
      this.GameState.RemoveEntityFromMap(inventoryEvent.Item);
      this.OnGameEvent(inventoryEvent.Entity.PickedUp(inventoryEvent.Item));
    }
  }

}
