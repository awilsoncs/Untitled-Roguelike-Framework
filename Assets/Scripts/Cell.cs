using UnityEngine;

/// <summary>
/// Cells provide a geometric container for game objects.
/// 
/// Cells do NOT provide any object management functionality.
/// </summary>
public class Cell {
    int x, y = 0;

    // this item takes up the majority of the space here.
    IEntity contents;

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public IEntity GetContents() {
        return contents;
    }

    public void PutContents(IEntity entity) {
        if (contents != null) {
            Debug.LogError("Attempted to set a filled cell!");
            return;
        }
        this.contents = entity;
    }

    public IEntity ClearContents() {
        IEntity entity = this.contents;
        this.contents = null;
        return entity;
    }

    public bool IsPassable() {
        return (contents == null || !contents.BlocksMove);
    }

    public bool IsTransparent() {
        return (contents == null || !contents.BlocksSight);
    }
}