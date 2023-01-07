namespace URF.Common.Entities {
  using System;
  using System.Collections.Generic;
  using URF.Common.Persistence;

  /// <summary>
  /// Represent Inventory information for an entity.
  /// </summary>
  public class InventoryComponent : BaseComponent {

    /// <summary>
    /// A list of entity IDs for entities contained within this object.
    /// </summary>
    public List<int> Contents {
      get;
    } = new();

    /// <summary>
    /// Add an entity to the inventory.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void Add(IEntity entity) {
      this.Contents.Add(entity.ID);
    }

    /// <summary>
    /// Remove an entity from the inventory.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(IEntity entity) {
      if (!this.Contents.Contains(entity.ID)) {
        throw new ArgumentException("Cannot remove item not in inventory");
      }
      _ = this.Contents.Remove(entity.ID);
    }

    /// <summary>
    /// Return whether this inventory contains the given entity.
    /// </summary>
    /// <param name="entity">The entity to query for</param>
    /// <returns>True if the inventory contains the entity, false otherwise</returns>
    public bool Contains(IEntity entity) {
      return this.Contents.Contains(entity.ID);
    }

    /// <inheritdoc>
    public override void Load(IGameDataReader reader) {
      this.Contents.Clear();
      int count = reader.ReadInt();
      for (int i = 0; i < count; i++) {
        this.Contents.Add(reader.ReadInt());
      }
    }

    /// <inheritdoc>
    public override void Save(IGameDataWriter writer) {
      int count = this.Contents.Count;
      writer.Write(count);
      for (int i = 0; i < count; i++) {
        writer.Write(this.Contents[i]);
      }
    }

  }
}
