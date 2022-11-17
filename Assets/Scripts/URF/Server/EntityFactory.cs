using System;
using System.Collections.Generic;
using URF.Common.Entities;
using URF.Server.RulesSystems;

namespace URF.Server {
  public class EntityFactory : IEntityFactory {

    private int _idCounter;

    private delegate void EntityBuilder(Entity entity);

    private readonly Dictionary<string, EntityBuilder> _builders;

    private readonly List<Type> _entitySpecComponents;

    public EntityFactory() {
      _entitySpecComponents = new List<Type>();
      _builders = new Dictionary<string, EntityBuilder> {
        { "player", BuildPlayer },
        { "crab", BuildCrab },
        { "wall", BuildWall },
        { "healthPotion", BuildHealthPotion }
      };
    }

    /// <summary>
    /// Get a bare entity.
    /// </summary>
    /// <returns></returns>
    public Entity Get() {
      Entity entity = new();
      foreach(Type t in _entitySpecComponents) {
        entity.AddComponent((BaseComponent)Activator.CreateInstance(t));
      }

      return entity;
    }

    /// <summary>
    /// Get an entity specified by a blueprint name.
    /// </summary>
    /// <param name="bluePrint">The string name of the entity's blueprint.</param>
    /// <returns>An entity conforming to the given blueprint.</returns>
    public Entity Get(string bluePrint) {
      Entity entity = Get();
      _builders[bluePrint](entity);
      entity.ID = _idCounter++;
      return entity;
    }

    public void UpdateEntitySpec(List<Type> componentTypes) {
      foreach(Type slot in componentTypes) { _entitySpecComponents.Add(slot); }
    }

    public void Reclaim(Entity entity) {
      // Haven't yet implemented this, but need to know where it should be called, so we leave it.
    }

    // short term hardcoded delegates
    private static void BuildPlayer(Entity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Player";
      info.Appearance = "player";
      info.Description = "A daring adventurer.";
      CombatComponent combat = entity.GetComponent<CombatComponent>();
      combat.CanFight = true;
      combat.CurrentHealth = 10;
      combat.MaxHealth = 10;
      combat.Damage = 2;
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = true;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.None;
      entity.BlocksSight = false;
      entity.IsVisible = true;
    }

    private static void BuildCrab(Entity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Crab";
      info.Appearance = "crab";
      info.Description = "A deadly crab.";
      CombatComponent combat = entity.GetComponent<CombatComponent>();
      combat.CanFight = true;
      combat.CurrentHealth = 2;
      combat.MaxHealth = 2;
      combat.Damage = 1;
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = true;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.Monster;
      entity.BlocksSight = false;
      entity.IsVisible = true;
    }

    private static void BuildWall(Entity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Wall";
      info.Appearance = "wall";
      info.Description = "Nothing but solid stone.";
      CombatComponent combat = entity.GetComponent<CombatComponent>();
      combat.CanFight = false;
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = true;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.None;
      entity.BlocksSight = true;
      entity.IsVisible = true;
    }

    private static void BuildHealthPotion(Entity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Health Potion";
      info.Appearance = "healthPotion";
      info.Description = "It looks like Diet Soda.";
      CombatComponent combat = entity.GetComponent<CombatComponent>();
      combat.CanFight = false;
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = false;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.None;
      entity.BlocksSight = false;
      entity.IsVisible = true;
    }

  }
}
