using System.Collections.Generic;
using System.Linq;
using URF.Server.GameState;
using URF.Server.RulesSystems;

namespace URF.Common.Entities
{
    /// <summary>
    /// Represent the logical component of a game piece. Should remain agnostic to the game engine.
    /// </summary>
    public class Entity : IPersistableObject, IEntity {
        public int ID { get; set; }
        public bool BlocksSight { get; set; }
        public bool IsVisible { get; set; }
        public IGameState GameState {get; set;}
        private readonly List<BaseComponent> components;

        public Entity() {
            components = new();
        }

        public T GetComponent<T>() where T : BaseComponent {
            return (T)components.FirstOrDefault(c => c is T);
        }

        public void AddComponent(BaseComponent component) {
            components.Add(component);
        }

        public void Save(GameDataWriter writer) {
            writer.Write(BlocksSight);
            writer.Write(IsVisible);
            writer.Write(components.Count);
            foreach (var component in components) {
                component.Save(writer);
            }
        }

        public void Load(GameDataReader reader) {
            BlocksSight = reader.ReadBool();
            IsVisible = reader.ReadBool();
            var componentCount = reader.ReadInt();
            for (int i = 0; i < componentCount; i++) {
                components[i].Load(reader);
            }
        }

        public void Recycle(IEntityFactory entityFactory) {
            entityFactory.Reclaim(this);
        }

        public override string ToString()
        {
            var movement = GetComponent<Movement>();
            var entityInfo = GetComponent<EntityInfo>();

            if (movement == null) {
                return $"{entityInfo.Name}::{ID}";
            } else {
                return $"{entityInfo.Name}::{ID}::{movement.EntityPosition})";
            }
        }

    }
}
