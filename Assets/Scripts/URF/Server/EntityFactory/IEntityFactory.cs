namespace URF.Server.EntityFactory {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;

  public interface IEntityFactory<TEntity> where TEntity : IEntity, new() {

    /// <summary>
    /// Get an base entity.
    /// </summary>
    /// <returns></returns>
    IEntity Get();

    /// <summary>
    /// Get an entity after applying a blueprint by the given name on it.
    /// </summary>
    /// <param name="bluePrint">the string name of a blueprint</param>
    /// <returns></returns>
    IEntity Get(string bluePrint);

    /// <summary>
    /// Reclaim the entity for object reuse.
    /// </summary>
    /// <param name="entity">the entity to be reclaimed</param>
    void Reclaim(TEntity entity);

    /// <summary>
    /// Apply a list of types to the entity specification.
    /// </summary>
    /// <param name="componentTypes">a list of types to apply to the specification</param>
    void UpdateEntitySpec(List<Type> componentTypes);

    /// <summary>
    /// Register a blueprint with the factory.
    /// </summary>
    /// <param name="bluePrintName">the name to reference the blueprint under</param>
    /// <param name="builder">
    /// a callback function that sets default values on the new entity</param>
    void RegisterBlueprint(string bluePrintName, EntityBuilder builder);

  }
}
