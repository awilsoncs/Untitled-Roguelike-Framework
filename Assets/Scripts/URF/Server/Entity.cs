namespace URF.Server {
  using System.Collections.Generic;
  using System.Linq;
  using URF.Common.Entities;
  using URF.Common.Persistence;
  using URF.Server.RulesSystems;

  /// <summary>
  /// Backing implementation for the IEntity. Only EntityFactory should have access to this.
  /// </summary>
  public class Entity : IEntity {

    public int ID {
      get; set;
    }

    public bool BlocksSight {
      get; set;
    }

    public bool IsVisible {
      get; set;
    }

    private readonly List<BaseComponent> components = new();

    public T GetComponent<T>() where T : BaseComponent {
      return (T)this.components.FirstOrDefault(c => c is T);
    }

    public void AddComponent(BaseComponent component) {
      this.components.Add(component);
    }

    public void Save(GameDataWriter writer) {
      writer.Write(this.BlocksSight);
      writer.Write(this.IsVisible);
      writer.Write(this.components.Count);
      foreach (BaseComponent component in this.components) {
        component.Save(writer);
      }
    }

    public void Load(GameDataReader reader) {
      this.BlocksSight = reader.ReadBool();
      this.IsVisible = reader.ReadBool();
      int componentCount = reader.ReadInt();
      for (int i = 0; i < componentCount; i++) {
        this.components[i].Load(reader);
      }
    }

    public override string ToString() {
      Movement movement = this.GetComponent<Movement>();
      EntityInfo entityInfo = this.GetComponent<EntityInfo>();

      return movement == null
        ? $"{entityInfo.Name}::{this.ID}"
        : $"{entityInfo.Name}::{this.ID}::{movement.EntityPosition})";
    }

  }
}
