namespace URF.Common.Entities {
  using URF.Common.Persistence;

  public abstract class BaseComponent : IPersistableObject {

    // The component's contribution to the entity's ToString value.
    public virtual string EntityString => "";

    public virtual void Load(IGameDataReader reader) {
      // the base component doesn't provide loading behavior
    }

    public virtual void Save(IGameDataWriter writer) {
      // the base component doesn't provide loading behavior
    }

  }
}
