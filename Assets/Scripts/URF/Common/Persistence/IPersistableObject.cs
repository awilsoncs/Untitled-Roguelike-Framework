namespace URF.Common.Persistence {
  public interface IPersistableObject {

    void Save(IGameDataWriter writer);

    void Load(IGameDataReader reader);

  }
}
