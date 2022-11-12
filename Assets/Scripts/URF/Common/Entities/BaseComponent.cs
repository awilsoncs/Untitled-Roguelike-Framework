using URF.Common.Persistence;

namespace URF.Common.Entities {
  public abstract class BaseComponent : IPersistableObject {

    public abstract void Load(GameDataReader reader);

    public abstract void Save(GameDataWriter writer);

  }
}