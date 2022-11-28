namespace URF.Server.GameState {
  using System.Collections.Generic;
  using System.Linq;
  using URF.Common.Entities;
  using URF.Server.RulesSystems;

  /// <summary>
  /// Cells provide a geometric container for game objects. Cells do NOT provide
  /// any object management functionality.
  /// </summary>
  public class Cell {

    public HashSet<IEntity> Contents {
      get;
    }

    public bool IsPassable => this.Contents.Count == 0 ||
          this.Contents.Select(x => x.GetComponent<Movement>()).All(x => !x.BlocksMove);


    public bool IsTransparent => this.Contents.Count == 0 || !this.Contents.Any(x => x.BlocksSight);


    public Cell() {
      this.Contents = new HashSet<IEntity>();
    }

    public void PutContents(IEntity entity) {
      _ = this.Contents.Add(entity);
    }

    public void RemoveEntity(IEntity entity) {
      if (this.Contents.Contains(entity)) {
        _ = this.Contents.Remove(entity);
      }
    }
  }
}
