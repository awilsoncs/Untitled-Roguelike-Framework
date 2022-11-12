using System;
using System.Collections.Generic;
using URF.Common.Entities;

namespace URF.Server.GameState
{
    public interface IEntityFactory {
        // todo implement pooling behavior
        Entity Get();
        Entity Get(String s);
        void Reclaim (Entity entity);
        void UpdateEntitySpec(List<Type> componentTypes);
    }
}