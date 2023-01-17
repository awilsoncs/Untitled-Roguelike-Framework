namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Effects;

  /// <summary>
  /// Defines inventory handling for entities.
  /// </summary>
  public class InventorySystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
            typeof(InventoryComponent)
      };

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
      }
    }

    private void HandleWantsToDrop(InventoryEvent inventoryEvent) {
      IEntity entity = inventoryEvent.Entity;
      InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
      if (inventory == null) {
        throw new GameEventException(
          inventoryEvent, "The acting entity does not have an InventoryComponent.");
      }

      IEntity item = inventoryEvent.Item;

      if (!inventory.Contains(item)) {
        throw new GameEventException(
          inventoryEvent, "The inventory doesn't contain the expected item.");
      }

      inventory.Remove(item);
      Position entityPos = this.GameState.LocateEntityOnMap(entity);
      this.GameState.PlaceEntityOnMap(item, entityPos);
      this.OnGameEvent(inventoryEvent.Entity.Dropped(item));
      return;
    }

    private void HandleWantsToUse(InventoryEvent inventoryEvent) {
      IEntity entity = inventoryEvent.Entity;
      InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
      if (inventory == null) {
        throw new GameEventException(
          inventoryEvent, "The acting entity does not have an InventoryComponent.");
      }

      IEntity item = inventoryEvent.Item;

      if (!inventory.Contains(item)) {
        throw new GameEventException(
          inventoryEvent, "The inventory doesn't contain the expected item.");
      }

      this.OnGameEvent(entity.Used(item));
      return;
    }

    private void HandleWantsToGet(InventoryEvent inventoryEvent) {
      this.GameState.RemoveEntityFromMap(inventoryEvent.Item);
      InventoryComponent inventory = inventoryEvent.Entity.GetComponent<InventoryComponent>();
      inventory.Add(inventoryEvent.Item);
      this.OnGameEvent(inventoryEvent.Entity.PickedUp(inventoryEvent.Item));
      return;
    }

    private void HandleUsed(InventoryEvent inventoryEvent) {
      IEntity entity = inventoryEvent.Entity;
      IEntity item = inventoryEvent.Item;
      InventoryComponent inventory = entity.GetComponent<InventoryComponent>();

      if (!inventory.Contains(item)) {
        throw new GameEventException(
          inventoryEvent, "The inventory doesn't contain the expected item.");
      }

      this.OnGameEvent(
        EffectEvent.Created(new RestoreHealthEffect(entity, 5))
      );

      inventory.Remove(item);
      this.GameState.DeleteEntity(item);
      return;
    }

  }

}
