namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  /// <summary>
  /// Defines inventory handling for entities.
  /// </summary>
  public class InventorySystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
            typeof(InventoryComponent)
      };

    public override void HandleInventoryEvent(InventoryEvent inventoryEvent) {
      if (inventoryEvent.Action != InventoryEvent.InventoryAction.WantsToGet) {
        return;
      }
      this.GameState.RemoveEntityFromMap(inventoryEvent.Item);
      InventoryComponent inventory = inventoryEvent.Entity.GetComponent<InventoryComponent>();
      inventory.Add(inventoryEvent.Item);
      this.OnGameEvent(inventoryEvent.Entity.PickedUp(inventoryEvent.Item));
    }
  }

}
