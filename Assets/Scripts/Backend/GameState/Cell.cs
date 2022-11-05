/// <summary>
/// Cells provide a geometric container for game objects. Cells do NOT provide
/// any object management functionality.
/// </summary>
public class Cell {
    // this item takes up the majority of the space here.
    IEntity contents;

    public IEntity GetContents() {
        return contents;
    }

    public void PutContents(IEntity entity) {
        if (contents != null) {
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
        return (contents == null || !contents.GetBoolSlot("blocksMove"));
    }

    public bool IsTransparent
    { 
        get {return (contents == null || !contents.BlocksSight);}
    }
}