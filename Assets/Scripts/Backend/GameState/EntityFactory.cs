using System;
using System.Collections.Generic;

public class EntityFactory : IEntityFactory {
// todo implement pooling behavior
    private int idCounter = 0;
    private delegate Entity EntityBuilder();
    
    Dictionary<String, EntityBuilder> constructors;

    public EntityFactory() {
        constructors = new Dictionary<string, EntityBuilder>();
        constructors.Add("player", this.GetPlayer);
        constructors.Add("crab", this.GetCrab);
        constructors.Add("wall", this.GetWall);
    }    

    /*
    * Get a bare entity.
    */
    public Entity Get() {
        return new Entity();
    }

    /*
    * Get an entity specified by a blueprint name.
    */
    public Entity Get(String s) {
        Entity entity = constructors[s]();
        entity.ID = idCounter++;
        return entity;
    }

    public void Reclaim (Entity entity) {}

    // short term hardcoded delegates
    Entity GetPlayer() {
        var entity = Get();
        entity.Name = "Player";
        entity.BlocksMove = true;
        entity.BlocksSight = false;
        entity.Appearance = "player";
        entity.IsVisible = true;
        return entity;
    }

    Entity GetCrab() {
        var entity = Get();
        entity.Name = "Crab";
        entity.BlocksMove = true;
        entity.BlocksSight = false;
        entity.Appearance = "crab";
        entity.IsVisible = true;
        return entity;
    }

    Entity GetWall() {
        var entity = Get();
        entity.Name = "Wall";
        entity.BlocksMove = true;
        entity.BlocksSight = true;
        entity.Appearance = "wall";
        entity.IsVisible = true;
        return entity;
    }
}