using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


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
        // mad science - saving components
        foreach (var component in components) {
            // Need to memo what kind of component this is
            var componentType = component.GetType();
            var attr = (ComponentAttribute) componentType
                .GetCustomAttributes(typeof(ComponentAttribute), false).Single();
            writer.Write(attr.guid);
            component.Save(writer);
        }
    }

    public void Load(GameDataReader reader) {
        BlocksSight = reader.ReadBool();
        IsVisible = reader.ReadBool();
        var componentCount = reader.ReadInt();
        for (int i = 0; i < componentCount; i++) {
            var typeGuid = reader.ReadString();
            // todo we don't really need to do this search
            // the components *should* always be in the same order
            // perhaps make this configurable
            foreach (var component in components) {
                // find the right component
                // todo optimize this probably
                var componentType = component.GetType();
                var attr = (ComponentAttribute) componentType
                    .GetCustomAttributes(typeof(ComponentAttribute), false).Single();
                if (attr.guid == typeGuid) {
                    component.Load(reader);
                    break;
                }
            }
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
            return $"{entityInfo.Name}::{ID}::{movement.Position})";
        }
    }

}
