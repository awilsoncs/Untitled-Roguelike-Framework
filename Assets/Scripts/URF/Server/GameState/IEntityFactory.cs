using System;
using System.Collections.Generic;
using URF.Common.Entities;

namespace URF.Server.GameState {
  public interface IEntityFactory {

    Entity Get();

    Entity Get(string bluePrint);

    void Reclaim(Entity entity);

    void UpdateEntitySpec(List<Type> componentTypes);

  }
}
