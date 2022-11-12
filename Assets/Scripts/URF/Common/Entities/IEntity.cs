using URF.Server.GameState;

namespace URF.Common.Entities
{
    /// <summary>
    /// High level container for logical game objects.
    /// </summary>
    public interface IEntity : IPersistableObject {
        /// <summary>
        /// The entity's unique identifier in the game world.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Whether the entity should block field of view.
        /// </summary>
        bool BlocksSight { get; set; }

        /// <summary>
        /// Whether the entity is currently visible to the player.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Recycle this entity at the given factory.
        /// </summary>
        /// <param name="entityFactory">A reference to a factory that can dispose 
        /// of this entity properly.</param>
        void Recycle(IEntityFactory entityFactory);
        T GetComponent<T>() where T : BaseComponent;
        void AddComponent(BaseComponent component);
    }
}