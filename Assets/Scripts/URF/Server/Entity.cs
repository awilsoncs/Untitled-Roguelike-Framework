namespace URF.Server {
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using URF.Common.Entities;
  using URF.Common.Persistence;

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

    public void Save(IGameDataWriter writer) {
      if (writer == null) {
        return;
      }
      writer.Write(this.BlocksSight);
      writer.Write(this.IsVisible);
      foreach (BaseComponent component in this.components) {
        component.Save(writer);
      }
    }

    public void Load(IGameDataReader reader) {
      if (reader == null) {
        return;
      }
      this.BlocksSight = reader.ReadBool();
      this.IsVisible = reader.ReadBool();
      foreach (BaseComponent component in this.components) {
        component.Load(reader);
      }
    }

    public override string ToString() {
      var stringBuilder = new StringBuilder();
      _ = stringBuilder.Append(this.ID);
      foreach (BaseComponent component in this.components) {
        string contribution = component.EntityString;
        if (!contribution.Equals("")) {
          _ = stringBuilder.Append($"::{contribution}");
        }
      }

      return stringBuilder.ToString();
    }

  }
}
