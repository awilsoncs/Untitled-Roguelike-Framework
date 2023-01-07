namespace URF.Common.Entities {
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
      _ = this.Contents.Remove(entity.ID);
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
