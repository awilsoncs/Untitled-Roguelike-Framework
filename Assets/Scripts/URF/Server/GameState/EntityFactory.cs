using System;
using System.Collections.Generic;
using URF.Common.Entities;
using URF.Server.GameState;
using URF.Server.RulesSystems;

public class EntityFactory : IEntityFactory {
// todo implement pooling behavior
    private int idCounter = 0;
    private delegate void EntityBuilder(Entity entity);
    
    readonly Dictionary<String, EntityBuilder> builders;

    readonly List<Type> entitySpecComponents;


    public EntityFactory() {
        entitySpecComponents = new();
        builders = new()
        {
            { "player", BuildPlayer },
            { "crab", BuildCrab },
            { "wall", BuildWall },
            { "healthPotion", BuildHealthPotion}
        };
    }    

    /*
    * Get a bare entity.
    */
    public Entity Get() {
        Entity entity = new();
        foreach (var t in entitySpecComponents) {
            entity.AddComponent((BaseComponent) Activator.CreateInstance(t));
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

    public void UpdateEntitySpec(List<Type> newComponents) {
        foreach (var slot in newComponents) {
            entitySpecComponents.Add(slot);
        }
    }

    public void Reclaim (Entity entity) {}

    // short term hardcoded delegates
    void BuildPlayer(Entity entity) {
        var info = entity.GetComponent<EntityInfo>();
        info.Name = "Player";
        info.Appearance = "player";
        info.Description = "A daring adventurer.";
        var combat = entity.GetComponent<CombatComponent>();
        combat.CanFight = true;
        combat.CurrentHealth = 10;
        combat.MaxHealth = 10;
        combat.Damage = 2;
        var movement = entity.GetComponent<Movement>();
        movement.BlocksMove = true;
        var brain = entity.GetComponent<Brain>();
        brain.ControlMode = IntelligenceControlMode.None;
        entity.BlocksSight = false;
        entity.IsVisible = true;
    }

    void BuildCrab(Entity entity) {
        var info = entity.GetComponent<EntityInfo>();
        info.Name = "Crab";
        info.Appearance = "crab";
        info.Description = "A deadly crab.";
        var combat = entity.GetComponent<CombatComponent>();
        combat.CanFight = true;
        combat.CurrentHealth = 2;
        combat.MaxHealth = 2;
        combat.Damage = 1;
        var movement = entity.GetComponent<Movement>();
        movement.BlocksMove = true;
        var brain = entity.GetComponent<Brain>();
        brain.ControlMode = IntelligenceControlMode.Monster;
        entity.BlocksSight = false;
        entity.IsVisible = true;
    }

    void BuildWall(Entity entity) {
        var info = entity.GetComponent<EntityInfo>();
        info.Name = "Wall";
        info.Appearance = "wall";
        info.Description = "Nothing but solid stone.";
        var combat = entity.GetComponent<CombatComponent>();
        combat.CanFight = false;
        var movement = entity.GetComponent<Movement>();
        movement.BlocksMove = true;
        var brain = entity.GetComponent<Brain>();
        brain.ControlMode = IntelligenceControlMode.None;
        entity.BlocksSight = true;
        entity.IsVisible = true;
    }

    void BuildHealthPotion(Entity entity) {
        var info = entity.GetComponent<EntityInfo>();
        info.Name = "Health Potion";
        info.Appearance = "healthPotion";
        info.Description = "It looks like Diet Soda.";
        var combat = entity.GetComponent<CombatComponent>();
        combat.CanFight = false;
        var movement = entity.GetComponent<Movement>();
        movement.BlocksMove = false;
        var brain = entity.GetComponent<Brain>();
        brain.ControlMode = IntelligenceControlMode.None;
        entity.BlocksSight = false;
        entity.IsVisible = true;
    }
}