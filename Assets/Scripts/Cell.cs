using UnityEngine;

/// <summary>
/// Cells provide a geometric container for game objects.
/// 
/// Cells do NOT provide any object management functionality.
/// </summary>
public class Cell {
    int x, y = 0;

    // this item takes up the majority of the space here.
    Entity contents;

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Entity GetContents() {
        return contents;
    }

    public void PutContents(Entity entity) {
        if (contents != null) {
            Debug.LogError("Attempted to set a full cell!");
            return;
        }
        this.contents = entity;
    }

    public Entity ClearContents() {
        Entity entity = this.contents;
        this.contents = null;
        return entity;
    }

    public bool IsPassable() {
        return (contents == null || !contents.BlocksMove);
    }
}