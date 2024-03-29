namespace URF.Server.EntityFactory {
  using System.Collections.Generic;
  using System.Diagnostics;
  using URF.Common.Entities;

  public class EntityFactory<TEntity> : IEntityFactory<TEntity> where TEntity : IEntity, new() {

    private int idCounter;

    private readonly Dictionary<string, EntityBuilder> builders;

    public EntityFactory() {
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
      return new TEntity();
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

    public void Reclaim(TEntity entity) {
      // Haven't yet implemented this, but need to know where it should be called, so we leave it.
    }

    public void RegisterBlueprint(string bluePrintName, EntityBuilder builder) {
      Debug.Assert(!this.builders.ContainsKey(bluePrintName));
      this.builders[bluePrintName] = builder;
    }

    // short term hardcoded delegates

    private static void BuildPlayer(IEntity entity) {
      entity.Name = "Player";
      entity.Appearance = "player";
      entity.Description = "A daring adventurer.";
      entity.BlocksMove = true;
      entity.ControlMode = ControlMode.None;
      entity.BlocksSight = false;
      entity.IsVisible = true;
      entity.CanFight = true;
      entity.CurrentHealth = 10;
      entity.MaxHealth = 10;
      entity.Damage = 2;
    }

    private static void BuildCrab(IEntity entity) {
      entity.Name = "Crab";
      entity.Appearance = "crab";
      entity.Description = "A deadly crab.";
      entity.BlocksMove = true;
      entity.ControlMode = ControlMode.Monster;
      entity.BlocksSight = false;
      entity.IsVisible = true;
      entity.CanFight = true;
      entity.CurrentHealth = 2;
      entity.MaxHealth = 2;
      entity.Damage = 1;
    }

    private static void BuildWall(IEntity entity) {
      entity.Name = "Wall";
      entity.Appearance = "wall";
      entity.Description = "Nothing but solid stone.";
      entity.BlocksMove = true;
      entity.ControlMode = ControlMode.None;
      entity.BlocksSight = true;
      entity.IsVisible = true;
      entity.CanFight = false;
    }

    private static void BuildHealthPotion(IEntity entity) {
      entity.Name = "Health Potion";
      entity.Appearance = "healthPotion";
      entity.Description = "It looks like Diet Soda.";
      entity.BlocksMove = false;
      entity.ControlMode = ControlMode.None;
      entity.BlocksSight = false;
      entity.IsVisible = true;
      entity.CanFight = false;
    }

  }
}
