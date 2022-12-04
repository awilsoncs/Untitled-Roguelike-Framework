namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class InventoryChanged : EventArgs, IGameEvent {

    public enum InventoryAction {
      PickedUp,
      Dropped,
      Consumed
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

    public InventoryChanged(IEntity entity, InventoryAction action, IEntity item) {
      this.Entity = entity;
      this.Action = action;
      this.Item = item;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleInventoryChanged(this);
    }
  }

  public static class InventoryChangedExtensions {
    public static InventoryChanged PickedUp(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryChanged(entity, InventoryChanged.InventoryAction.PickedUp, other);
    }

    public static InventoryChanged Dropped(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryChanged(entity, InventoryChanged.InventoryAction.Dropped, other);
    }

    public static InventoryChanged Consumed(
      this IEntity entity,
      IEntity other
    ) {
      return new InventoryChanged(entity, InventoryChanged.InventoryAction.Consumed, other);
    }
  }

}
