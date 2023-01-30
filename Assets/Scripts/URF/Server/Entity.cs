namespace URF.Server {
  using System.Collections.Generic;
  using System.Linq;
  using URF.Common.Entities;
  using URF.Common.Persistence;
  using URF.Server.Entities;

  /// <summary>
  /// Backing implementation for the IEntity. Only EntityFactory should have access to this.
  /// </summary>
  public class Entity : IEntity {

    public int ID {
      get; set;
    }

    public bool BlocksMove {
      get; set;
    }

    public bool BlocksSight {
      get; set;
    }

    public bool IsVisible {
      get; set;
    }

    public bool CanFight {
      get; set;
    }

    public int MaxHealth {
      get; set;
    }

    public int CurrentHealth {
      get; set;
    }

    public int Damage {
      get; set;
    }

    public List<int> Inventory {
      get;
    } = new();

    public string Name {
      get; set;
    }

    public string Appearance {
      get; set;
    }

    public string Description {
      get; set;
    }

    public ControlMode ControlMode {
      get; set;
    }

    public List<IEntity> VisibleEntities {
      get;
    } = new();

    public ICombatStats CombatStats => this.combatStats;

    private readonly CombatStats combatStats = new();

    public IUseableInfo UseableInfo => this.useableInfo;

    private readonly UseableInfo useableInfo = new();


    public void Save(IGameDataWriter writer) {
      if (writer == null) {
        return;
      }
      writer.Write(this.Name);
      writer.Write((int)this.ControlMode);
      writer.Write(this.Appearance);
      writer.Write(this.Description);
      writer.Write(this.BlocksSight);
      writer.Write(this.BlocksMove);
      writer.Write(this.IsVisible);
      writer.Write(this.CanFight);
      writer.Write(this.CurrentHealth);
      writer.Write(this.MaxHealth);
      writer.Write(this.Damage);
      writer.Write(this.Inventory.Count());
      foreach (int itemId in this.Inventory) {
        writer.Write(itemId);
      }

      this.combatStats.Save(writer);
      this.useableInfo.Save(writer);
    }

    public void Load(IGameDataReader reader) {
      if (reader == null) {
        return;
      }
      this.Name = reader.ReadString();
      this.ControlMode = (ControlMode)reader.ReadInt();
      this.Appearance = reader.ReadString();
      this.Description = reader.ReadString();
      this.BlocksSight = reader.ReadBool();
      this.BlocksMove = reader.ReadBool();
      this.IsVisible = reader.ReadBool();
      this.CanFight = reader.ReadBool();
      this.CurrentHealth = reader.ReadInt();
      this.MaxHealth = reader.ReadInt();
      this.Damage = reader.ReadInt();
      this.VisibleEntities.Clear();

      this.Inventory.Clear();
      int inventoryCount = reader.ReadInt();
      for (int i = 0; i < inventoryCount; i++) {
        this.Inventory.Add(reader.ReadInt());
      }

      this.combatStats.Load(reader);
      this.useableInfo.Load(reader);
    }

    public override string ToString() {
      return $"{this.ID}::{this.Name}";
    }

  }
}
