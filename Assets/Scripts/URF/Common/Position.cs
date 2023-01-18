namespace URF.Common {
  /// <summary>
  /// Represent a point in the 2D game world.
  /// </summary>
  public readonly struct Position {

    /// <summary>
    /// The horizontal coordinate
    /// </summary>
    public int X {
      get;
    }


    /// <summary>
    /// The vertical coordinate
    /// </summary>
    public int Y {
      get;
    }

    public Position(int x, int y) {
      this.X = x;
      this.Y = y;
    }

    /// <summary>
    /// Return whether this position is possible in a 2D natural number matrix
    /// </summary>
    public bool IsValidPosition => this.X >= 0 && this.Y >= 0;

    public static implicit operator Position((int, int) t) => new(t.Item1, t.Item2);

    public void Deconstruct(out int x, out int y) {
      x = this.X;
      y = this.Y;
    }

    public override string ToString() {
      return $"({this.X},{this.Y})";
    }

    /// <summary>
    /// Add two positions using vector addition
    /// </summary>
    public static Position operator +(Position first, Position second)
      => (first.X + second.X, first.Y + second.Y);

    /// <summary>
    /// Provide the invalid position
    /// </summary>
    public static Position Invalid = new(-1, -1);

    public override bool Equals(object obj) {
      return (obj is Position position) && this.Equals(position);
    }

    public bool Equals(Position other) {
      return this.X == other.X && this.Y == other.Y;
    }

    public override int GetHashCode() {
      return System.HashCode.Combine(this.X, this.Y);
    }
  }
}
