namespace URF.Server.GameState {
  using System.Collections.Generic;
  using System.Linq;
  using URF.Common.Entities;
  using URF.Common.GameState;

  /// <summary>
  /// Cells provide a geometric container for game objects. Cells do NOT provide
  /// any object management functionality.
  /// </summary>
  public class Cell : IReadOnlyCell {
    public HashSet<IEntity> Contents { get; } = new();

    IReadOnlyCollection<IEntity> IReadOnlyCell.Contents => this.Contents;

    public bool IsTraversable => this.Contents.Count == 0 ||
          this.Contents.All(x => !x.BlocksMove);

    public bool IsTransparent => this.Contents.Count == 0 || !this.Contents.Any(x => x.BlocksSight);


    public void PutContents(IEntity entity) {
      _ = this.Contents.Add(entity);
    }

    public void RemoveEntity(IEntity entity) {
      if (this.Contents.Contains(entity)) {
        _ = this.Contents.Remove(entity);
      }
    }

    public override string ToString() {
      return $"Cell({this.Contents.Count()})";
    }
  }
}
