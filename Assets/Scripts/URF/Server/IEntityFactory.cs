using System;
using System.Collections.Generic;

namespace URF.Server {
  public interface IEntityFactory {

    Entity Get();

    Entity Get(string bluePrint);

    void Reclaim(Entity entity);

    void UpdateEntitySpec(List<Type> componentTypes);

  }
}
