namespace URF.Server.Entities {
  using System;
  using URF.Common.Entities;
  using URF.Common.Persistence;

  public class CombatStats : ICombatStats, IPersistableObject {

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

    public void Load(IGameDataReader reader) {
      if (reader == null) {
        throw new ArgumentNullException(nameof(reader));
      }
      this.CanFight = reader.ReadBool();
      if (!this.CanFight) {
        // A high % of entities can't fight, so we can save some disk space by not saving
        return;
      }

      this.MaxHealth = reader.ReadInt();
      this.CurrentHealth = reader.ReadInt();
      this.Damage = reader.ReadInt();
    }

    public void Save(IGameDataWriter writer) {
      if (writer == null) {
        throw new ArgumentNullException(nameof(writer));
      }
      writer.Write(this.CanFight);
      if (!this.CanFight) {
        // A high % of entities can't fight, so we can save some disk space by not saving
        return;
      }

      writer.Write(this.MaxHealth);
      writer.Write(this.CurrentHealth);
      writer.Write(this.Damage);
    }
  }
}
