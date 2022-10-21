using System;

public interface IEntityFactory {
// todo implement pooling behavior
    Entity Get();
    Entity Get(String s);
    void Reclaim (Entity entity);
}