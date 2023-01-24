namespace URF.Server.RulesSystems {
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.Exceptions;
  using URF.Common.GameEvents;
  using URF.Server.Resolvables;

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

      Resolvable resolvable = CreateResolvable(entity);
      this.BeginTargeting(resolvable);

      // todo this will delete the potion even if the user cancels.
      _ = entity.Inventory.Remove(item.ID);
      this.GameState.DeleteEntity(item);
    }

    private static Resolvable CreateResolvable(IEntity entity) {
      // Generate a resolvable for the item or ability and user.
      // todo we'll want this to read information off of the item or ability eventually
      return new Resolvable(
        entity,
        TargetScope.Self,
        new HashSet<IEffect>() { EffectType.RestoreHealth.WithMagnitude(5) }
      );
    }

    private void BeginTargeting(Resolvable resolvable) {
      // Emit the resolvable event for targeting.
      this.OnGameEvent(
        new ResolvableEvent(resolvable)
      );
    }
  }

}
