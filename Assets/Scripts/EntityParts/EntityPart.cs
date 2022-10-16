using UnityEngine;

public abstract class EntityPart  
#if UNITY_EDITOR
// Support pool rebuilding on editor hot reload. See EntityPart::OnEnable
: ScriptableObject 
#endif
 {
    public int ID { get; set; }
    public EntityPartType PartType { get; set; }

    // The owner entity
    public Entity Entity {get; set;}

    public virtual void GameUpdate() {}
    public abstract void Recycle();

    public abstract void Load(GameDataReader reader);
    public abstract void Save(GameDataWriter writer);

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