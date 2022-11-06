using System;
using System.Collections.Generic;

public class EntityFactory : IEntityFactory {
// todo implement pooling behavior
    private int idCounter = 0;
    private delegate void EntityBuilder(Entity entity);
    
    readonly Dictionary<String, EntityBuilder> builders;
    readonly HashSet<(string, SlotType)> entitySpec;

    public EntityFactory() {
        entitySpec = new();
        builders = new()
        {
            { "player", BuildPlayer },
            { "crab", BuildCrab },
            { "wall", BuildWall }
        };
    }    

    /*
    * Get a bare entity.
    */
    public Entity Get() {
        Entity entity = new();
        foreach (var slot in entitySpec) {
            switch (slot.Item2) {
                case SlotType.String:
                    entity.SetSlot(slot.Item1, "");
                    continue;
                case SlotType.Integer:
                    entity.SetSlot(slot.Item1, 0);
                    continue;
                case SlotType.Boolean:
                    entity.SetSlot(slot.Item1, false);
                    continue;
            }
        }
        return entity;
    }

    /*
    * Get an entity specified by a blueprint name.
    */
    public Entity Get(String s) {
        Entity entity = Get();
        builders[s](entity);
        entity.ID = idCounter++;
        return entity;
    }

    public void UpdateEntitySpec(List<(string, SlotType)> newSlots) {
        foreach (var slot in newSlots) {
            entitySpec.Add(slot);
        }
    }

    public void Reclaim (Entity entity) {}

    // short term hardcoded delegates
    void BuildPlayer(Entity entity) {
        entity.SetSlot("name", "Player");
        entity.SetSlot("canFight", true);
        entity.SetSlot("currentHealth", 10);
        entity.SetSlot("maxHealth", 10);
        entity.SetSlot("damage", 2);
        entity.SetSlot("blocksMove", true);
        entity.SetSlot("controlMethod", "player");
        entity.SetSlot("appearance", "player");
        entity.BlocksSight = false;
        entity.IsVisible = true;
    }

    void BuildCrab(Entity entity) {
        entity.SetSlot("name", "Crab");
        entity.SetSlot("canFight", true);
        entity.SetSlot("currentHealth", 2);
        entity.SetSlot("maxHealth", 2);
        entity.SetSlot("damage", 1);
        entity.SetSlot("controlMethod", "monster");
        entity.SetSlot("blocksMove", true);
        entity.SetSlot("appearance", "crab");
        entity.BlocksSight = false;
        entity.IsVisible = true;
    }

    void BuildWall(Entity entity) {
        entity.SetSlot("name", "Wall");
        entity.SetSlot("blocksMove", true);
        entity.SetSlot("appearance", "wall");
        entity.BlocksSight = true;
        entity.IsVisible = true;
    }
}