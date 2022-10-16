using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu]
public class EntityFactory : ScriptableObject {
// todo implement pooling behavior
    private int idCounter = 0;

    private delegate Entity EntityBuilder();
    [SerializeField] List<Sprite> sprites;
    
    Dictionary<String, EntityBuilder> constructors;

    public EntityFactory() {
        constructors = new Dictionary<string, EntityBuilder>();
        constructors.Add("player", this.GetPlayer);
        constructors.Add("crab", this.GetCrab);
    }    

    /*
    * Get a bare entity.
    */
    public Entity Get() {
        var gameObject = new GameObject();
        gameObject.name = "base entity";
        gameObject.AddComponent<SpriteRenderer>();
        var entity = gameObject.AddComponent<Entity>();
        return entity;
    }

    /*
    * Get an entity specified by a blueprint name.
    */
    public Entity Get(String s) {
        Entity entity = constructors[s]();
        entity.ID = idCounter++;
        return entity;
    }

    public Sprite GetSpriteByIndex(int i) {
        return sprites[i];
    }

    public void Reclaim (Entity entity) {
        Destroy(entity.gameObject);
    }

    // short term hardcoded delegates
    Entity GetPlayer() {
        Debug.Log("Entity factory producing a Player!");
        var entity = Get();
        entity.name = "Player";
        entity.AddPart(EntityPartPool<PlayerBrain>.Get());
        entity.SetSprite(sprites[0]);
        entity.SpriteIndex = 0;
        return entity;
    }

    Entity GetCrab() {
        var entity = Get();
        entity.name = "Crab";
        entity.SetSprite(sprites[1]);
        entity.SpriteIndex = 1;
        return entity;
    }
}