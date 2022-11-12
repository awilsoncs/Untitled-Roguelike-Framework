using System;
using System.Collections.Generic;
using URF.Common.Entities;

namespace URF.Server.RulesSystems {
  /// <summary>
  /// Defines basic meta information about entities.
  /// </summary>
  public class EntityInfoSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        // todo could create an annotation to register these
        typeof(EntityInfo)
      };

  }

  public class EntityInfo : BaseComponent {

    public string Name { get; set; }

    public string Appearance { get; set; }

    public string Description { get; set; }

    public override void Load(GameDataReader reader) {
      Name = reader.ReadString();
      Appearance = reader.ReadString();
      Description = reader.ReadString();
    }

    public override void Save(GameDataWriter writer) {
      writer.Write(Name);
      writer.Write(Appearance);
      writer.Write(Description);
    }

  }
}
