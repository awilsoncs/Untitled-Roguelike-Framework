using System.Collections.Generic;
using System.Linq;
using URF.Common.Entities;
using URF.Server.RulesSystems;

namespace URF.Server.GameState {
  /// <summary>
  /// Cells provide a geometric container for game objects. Cells do NOT provide
  /// any object management functionality.
  /// </summary>
  public class Cell {

    public HashSet<IEntity> Contents { get; }

    public Cell() {
      Contents = new HashSet<IEntity>();
    }

    public void PutContents(IEntity entity) {
      Contents.Add(entity);
    }

    public void RemoveEntity(IEntity entity) {
      if(Contents.Contains(entity)) { Contents.Remove(entity); }
    }

    public bool IsPassable {
      get {
        return Contents.Count == 0 ||
          Contents.Select(x => x.GetComponent<Movement>()).All(x => !x.BlocksMove);
      }
    }

    public bool IsTransparent {
      get { return Contents.Count == 0 || !Contents.Any(x => x.BlocksSight); }
    }

  }
}
