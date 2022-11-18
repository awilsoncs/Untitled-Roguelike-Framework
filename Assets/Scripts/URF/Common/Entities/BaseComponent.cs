using URF.Common.Persistence;

namespace URF.Common.Entities {
  public abstract class BaseComponent : IPersistableObject {

    public virtual void Load(GameDataReader reader) {
      // Default no-op;
    }

    public virtual void Save(GameDataWriter writer) {
      // Default no-op
    }

  }
}
