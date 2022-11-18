using System.Collections.Generic;
using System.Linq;
using URF.Common.Entities;
using URF.Common.Persistence;
using URF.Server.EntityFactory;
using URF.Server.RulesSystems;

namespace URF.Server {
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

    private readonly List<BaseComponent> _components = new();

    public T GetComponent<T>() where T : BaseComponent {
      return (T)_components.FirstOrDefault(c => c is T);
    }

    public void AddComponent(BaseComponent component) {
      _components.Add(component);
    }

    public void Save(GameDataWriter writer) {
      writer.Write(BlocksSight);
      writer.Write(IsVisible);
      writer.Write(_components.Count);
      foreach (BaseComponent component in _components) {
        component.Save(writer);
      }
    }

    public void Load(GameDataReader reader) {
      BlocksSight = reader.ReadBool();
      IsVisible = reader.ReadBool();
      int componentCount = reader.ReadInt();
      for (int i = 0; i < componentCount; i++) {
        _components[i].Load(reader);
      }
    }

    public void Recycle(IEntityFactory<Entity> entityFactory) {
      _components.Clear();
    }

    public override string ToString() {
      Movement movement = GetComponent<Movement>();
      EntityInfo entityInfo = GetComponent<EntityInfo>();

      return movement == null
        ? $"{entityInfo.Name}::{ID}"
        : $"{entityInfo.Name}::{ID}::{movement.EntityPosition})";
    }

  }
}
