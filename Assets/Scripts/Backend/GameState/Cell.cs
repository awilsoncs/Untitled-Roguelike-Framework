using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Cells provide a geometric container for game objects. Cells do NOT provide
/// any object management functionality.
/// </summary>
public class Cell {
    public List<IEntity> Contents {get;} 

    public Cell() {
        Contents = new List<IEntity>();
    }

    public void PutContents(IEntity entity) {
        Contents.Add(entity);
    }

    public IEntity ClearContents() {
        if (Contents.Count == 0) {
            return null;
        }

        IEntity entity = Contents[0];
        Contents.Clear();

        return entity;
    }

    public void RemoveEntity(IEntity entity) {
        if (Contents.Contains(entity)) {
            Contents.Remove(entity);
        }
    }

    public bool IsPassable {
        get {
            return Contents.Count == 0 ||
            Contents
            .Select(x => x.GetComponent<Movement>())
            .All(x => !x.BlocksMove);
        }
    }

    public bool IsTransparent { 
        get {
            return Contents.Count == 0 ||
            !Contents.Any(x => x.BlocksSight);
        }
    }
}