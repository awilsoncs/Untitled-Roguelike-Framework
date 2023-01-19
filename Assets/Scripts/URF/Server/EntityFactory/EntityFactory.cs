namespace URF.Server.EntityFactory {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using URF.Common.Entities;
  using URF.Server.RulesSystems;

  public class EntityFactory<TEntity> : IEntityFactory<TEntity> where TEntity : IEntity, new() {

    private int idCounter;


    private readonly Dictionary<string, EntityBuilder> builders;

    private readonly List<Type> entitySpecComponents;

    public EntityFactory() {
      this.entitySpecComponents = new List<Type>();
      this.builders = new Dictionary<string, EntityBuilder> {
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
    public IEntity Get() {
      var entity = new TEntity();
      foreach (Type t in this.entitySpecComponents) {
        entity.AddComponent((BaseComponent)Activator.CreateInstance(t));
      }

      return entity;
    }

    /// <summary>
    /// Get an entity specified by a blueprint name.
    /// </summary>
    /// <param name="bluePrint">The string name of the entity's blueprint.</param>
    /// <returns>An entity conforming to the given blueprint.</returns>
    public IEntity Get(string bluePrint) {
      IEntity entity = this.Get();
      this.builders[bluePrint](entity);
      entity.ID = this.idCounter++;
      return entity;
    }

    public void UpdateEntitySpec(List<Type> componentTypes) {
      foreach (Type slot in componentTypes) {
        this.entitySpecComponents.Add(slot);
      }
    }

    public void Reclaim(TEntity entity) {
      // Haven't yet implemented this, but need to know where it should be called, so we leave it.
    }

    public void RegisterBlueprint(string bluePrintName, EntityBuilder builder) {
      Debug.Assert(!this.builders.ContainsKey(bluePrintName));
      this.builders[bluePrintName] = builder;
    }

    // short term hardcoded delegates

    private static void BuildPlayer(IEntity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Player";
      info.Appearance = "player";
      info.Description = "A daring adventurer.";
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = true;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.None;
      entity.BlocksSight = false;
      entity.IsVisible = true;
      entity.CanFight = true;
      entity.CurrentHealth = 10;
      entity.MaxHealth = 10;
      entity.Damage = 2;
    }

    private static void BuildCrab(IEntity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Crab";
      info.Appearance = "crab";
      info.Description = "A deadly crab.";
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = true;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.Monster;
      entity.BlocksSight = false;
      entity.IsVisible = true;
      entity.CanFight = true;
      entity.CurrentHealth = 2;
      entity.MaxHealth = 2;
      entity.Damage = 1;
    }

    private static void BuildWall(IEntity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Wall";
      info.Appearance = "wall";
      info.Description = "Nothing but solid stone.";
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = true;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.None;
      entity.BlocksSight = true;
      entity.IsVisible = true;
      entity.CanFight = false;
    }

    private static void BuildHealthPotion(IEntity entity) {
      EntityInfo info = entity.GetComponent<EntityInfo>();
      info.Name = "Health Potion";
      info.Appearance = "healthPotion";
      info.Description = "It looks like Diet Soda.";
      Movement movement = entity.GetComponent<Movement>();
      movement.BlocksMove = false;
      Brain brain = entity.GetComponent<Brain>();
      brain.ControlMode = IntelligenceControlMode.None;
      entity.BlocksSight = false;
      entity.IsVisible = true;
      entity.CanFight = false;
    }

  }
}
