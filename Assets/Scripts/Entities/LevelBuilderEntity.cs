using UnityEngine;

// This pseudo-entity builds out the level.
public class LevelBuilderEntity : Entity {

    public LevelBuilderEntity(int id) : base(id) {}
    // Build out the level, then remove myself.
    public override void GameUpdate() {
        Debug.Log("Setting up level...");
    }
}