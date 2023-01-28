namespace URF.Server.Entities {
  using System;
  using URF.Common.Entities;
  using URF.Common.Persistence;
  using URF.Common.Useables;
  using URF.Server.Useables;

  public class UseableInfo : IUseableInfo, IPersistableObject {
    public bool IsUseable => this.Useable != null;

    public IUseable Useable {
      get; set;
    }

    public void Load(IGameDataReader reader) {
      if (reader == null) {
        throw new ArgumentNullException(nameof(reader));
      }
      bool isUseable = reader.ReadBool();
      if (isUseable) {
        this.Useable = new Useable();
        this.Useable.Load(reader);
      } else {
        this.Useable = null;
      }
    }

    public void Save(IGameDataWriter writer) {
      if (writer == null) {
        throw new ArgumentNullException(nameof(writer));
      }
      writer.Write(this.IsUseable);
      if (this.IsUseable) {
        this.Useable.Save(writer);
      }
    }
  }
}
