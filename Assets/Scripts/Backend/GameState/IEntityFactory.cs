using System;
using System.Collections.Generic;

public interface IEntityFactory {
// todo implement pooling behavior
    Entity Get();
    Entity Get(String s);
    void Reclaim (Entity entity);
    void UpdateEntitySpec(List<Type> componentTypes);
}