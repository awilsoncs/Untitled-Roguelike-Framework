namespace URF.Common.Entities {
  using URF.Common.Useables;

  public interface IUseableInfo {
    bool IsUseable {
      get;
    }

    IUseable Useable {
      get; set;
    }
  }
}
