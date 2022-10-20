using UnityEngine;

public interface IEntityPart : IPersistableObject
 {
    int ID { get; set; }
    EntityPartType PartType {get;}

    // The owner entity
    IEntity Entity {get; set;}

    void GameUpdate(IBoardController bc) {}
    void Recycle();

#if UNITY_EDITOR
    // marked by pools to indicate this object has been reclaimed
    // see OnEnable.
    bool IsReclaimed { get; set; }

    void OnEnable () {}
#endif
}