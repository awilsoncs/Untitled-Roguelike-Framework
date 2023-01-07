namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class InventoryEvent : EventArgs, IGameEvent {

    public enum InventoryAction {
      PickedUp,
      Dropped,
      Consumed,
      WantsToGet,
      WantsToDrop

    }

    /// <summary>The entity that picked the item up.</summary>
    public IEntity Entity {
      get;
    }

    public InventoryAction Action {
      get;
    }

    /// <summary>The item entity that was picked up.</summary>
    public IEntity Item {
      get;
    }

    public InventoryEvent(IEntity entity, InventoryAction action, IEntity item) {
      this.Entity = entity;
      this.Action = action;
      this.Item = item;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleInventoryEvent(this);
    }
  }

  public static class InventoryChangedExtensions {
    public static InventoryEvent PickedUp(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryEvent(entity, InventoryEvent.InventoryAction.PickedUp, other);
    }

    public static InventoryEvent Dropped(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryEvent(entity, InventoryEvent.InventoryAction.Dropped, other);
    }

    public static InventoryEvent Consumed(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryEvent(entity, InventoryEvent.InventoryAction.Consumed, other);
    }

    public static InventoryEvent WantsToGet(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryEvent(entity, InventoryEvent.InventoryAction.WantsToGet, other);
    }

    public static InventoryEvent WantsToDrop(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryEvent(entity, InventoryEvent.InventoryAction.WantsToDrop, other);
    }
  }

}
