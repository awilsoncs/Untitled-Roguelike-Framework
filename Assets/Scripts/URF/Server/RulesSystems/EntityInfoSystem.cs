namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Common.Persistence;

  /// <summary>
  /// Defines basic meta information about entities.
  /// </summary>
  public class EntityInfoSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        typeof(EntityInfo)
      };

  }

  public class EntityInfo : BaseComponent {

    public string Name {
      get; set;
    }

    public string Appearance {
      get; set;
    }

    public string Description {
      get; set;
    }

    public override void Load(IGameDataReader reader) {
      this.Name = reader.ReadString();
      this.Appearance = reader.ReadString();
      this.Description = reader.ReadString();
    }

    public override void Save(IGameDataWriter writer) {
      writer.Write(this.Name);
      writer.Write(this.Appearance);
      writer.Write(this.Description);
    }

  }
}
