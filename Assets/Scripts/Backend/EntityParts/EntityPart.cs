using UnityEngine;

// 
public abstract class EntityPart : 
#if UNITY_EDITOR
// Support pool rebuilding on editor hot reload. See EntityPart::OnEnable
ScriptableObject,
#endif
IEntityPart
 {
    public int Id { get; set; }
    public abstract EntityPartType PartType {get;}
    public IEntity Entity {get; set;}
    public virtual void GameUpdate(IGameState gs) {}
    public abstract void Recycle();
    public virtual void Load(GameDataReader reader) {
        Id = reader.ReadInt();
    }

    public virtual void Save(GameDataWriter writer) {
        writer.Write(Id);
    }

#if UNITY_EDITOR
    // marked by pools to indicate this object has been reclaimed
    // see OnEnable.
    public bool IsReclaimed { get; set; }

    void OnEnable () {
        // Will not be invoked in non-editor builds due to condition ScriptableObject
        // inheritance
        if (IsReclaimed) {
            // When we deserialize after a hot reload, we will have lost
            // all references in the behavior pools. Recycling previously
            // reclaimed behaviors allows the pools to rebuild automatically. 
            Recycle();
        }
    }
#endif
}