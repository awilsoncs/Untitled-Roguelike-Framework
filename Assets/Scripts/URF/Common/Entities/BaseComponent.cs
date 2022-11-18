namespace URF.Common.Entities {
  using URF.Common.Persistence;

  public abstract class BaseComponent : IPersistableObject {

    public virtual void Load(GameDataReader reader) {
      // the base component doesn't provide loading behavior
    }

    public virtual void Save(GameDataWriter writer) {
      // the base component doesn't provide loading behavior
    }

  }
}
