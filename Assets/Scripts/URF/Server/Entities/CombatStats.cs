namespace URF.Server.Entities {
  using URF.Common.Entities;
  using URF.Common.Persistence;

  public class CombatStats : ICombatStats, IPersistableObject {

    public bool CanFight {
      get; set;
    } = false;

    public int MaxHealth {
      get; set;
    } = 0;

    public int CurrentHealth {
      get; set;
    } = 0;

    public int Damage {
      get; set;
    } = 0;

    public void Load(IGameDataReader reader) {
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
