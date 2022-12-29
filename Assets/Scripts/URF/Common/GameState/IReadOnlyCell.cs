namespace URF.Common.GameState {
  using System.Collections.Generic;
  using URF.Common.Entities;

  public interface IReadOnlyCell {
    IReadOnlyCollection<IEntity> Contents {
      get;
    }

    bool IsTraversable {
      get;
    }


    bool IsTransparent {
      get;
    }
  }
}
