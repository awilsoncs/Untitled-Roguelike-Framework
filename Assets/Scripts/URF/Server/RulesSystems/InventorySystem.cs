namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.Exceptions;
  using URF.Common.GameEvents;

  /// <summary>
  /// Defines inventory handling for entities.
  /// </summary>
  public class InventorySystem : BaseRulesSystem {

    /// <inheritdoc />
    public override void HandleInventoryEvent(InventoryEvent inventoryEvent) {
      if (inventoryEvent == null) {
        throw new GameEventException(
          inventoryEvent, "HandleInventoryEvent called with null event.");
      } else if (inventoryEvent.Entity == null) {
        throw new GameEventException(
          inventoryEvent, "InventoryEvent must specify acting entity.");
      } else if (inventoryEvent.Item == null) {
        throw new GameEventException(
          inventoryEvent, "InventoryEvent must specify target entity");
      } else {
        // no missing fields!
      }

      switch (inventoryEvent.Action) {
        case InventoryEvent.InventoryAction.WantsToGet:
          this.HandleWantsToGet(inventoryEvent);
          return;
        case InventoryEvent.InventoryAction.WantsToDrop:
          this.HandleWantsToDrop(inventoryEvent);
          return;
        case InventoryEvent.InventoryAction.WantsToUse:
          this.HandleWantsToUse(inventoryEvent);
          return;
        case InventoryEvent.InventoryAction.Used:
          this.HandleUsed(inventoryEvent);
          return;
        default:
          return;
      }
    }

    private void HandleWantsToDrop(InventoryEvent inventoryEvent) {
      IEntity entity = inventoryEvent.Entity;
      if (entity.Inventory == null) {
        throw new GameEventException(
          inventoryEvent, "The acting entity does not have an InventoryComponent.");
      }

      IEntity item = inventoryEvent.Item;

      if (!entity.Inventory.Contains(item.ID)) {
        throw new GameEventException(
          inventoryEvent, "The inventory doesn't contain the expected item.");
      }

      _ = entity.Inventory.Remove(item.ID);
      Position entityPos = this.GameState.LocateEntityOnMap(entity);
      this.GameState.PlaceEntityOnMap(item, entityPos);
      this.OnGameEvent(inventoryEvent.Entity.Dropped(item));
    }

    private void HandleWantsToUse(InventoryEvent inventoryEvent) {
      IEntity entity = inventoryEvent.Entity;
      if (entity.Inventory == null) {
        throw new GameEventException(
          inventoryEvent, "The acting entity does not have an InventoryComponent.");
      }

      IEntity item = inventoryEvent.Item;

      if (!entity.Inventory.Contains(item.ID)) {
        throw new GameEventException(
          inventoryEvent, "The inventory doesn't contain the expected item.");
      }

      this.OnGameEvent(entity.Used(item));
    }

    private void HandleWantsToGet(InventoryEvent inventoryEvent) {
      this.GameState.RemoveEntityFromMap(inventoryEvent.Item);
      IEntity entity = inventoryEvent.Entity;
      entity.Inventory.Add(inventoryEvent.Item.ID);
      this.OnGameEvent(inventoryEvent.Entity.PickedUp(inventoryEvent.Item));
    }

    private void HandleUsed(InventoryEvent inventoryEvent) {
      IEntity entity = inventoryEvent.Entity;
      IEntity item = inventoryEvent.Item;

      if (!entity.Inventory.Contains(item.ID)) {
        throw new GameEventException(
          inventoryEvent, "The inventory doesn't contain the expected item.");
      }

      this.OnGameEvent(new EffectEvent(EffectEvent.EffectType.RestoreHealth, 5, entity));

      _ = entity.Inventory.Remove(item.ID);
      this.GameState.DeleteEntity(item);
    }

  }

}
