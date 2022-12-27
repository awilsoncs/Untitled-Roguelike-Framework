namespace URF.Common {
  public readonly struct Position {

    public int X { get; }

    public int Y { get; }

    public Position(int x, int y) {
      X = x;
      Y = y;
    }

    public bool IsValidPosition => this.X >= 0 && this.Y >= 0;

    public static implicit operator Position((int, int) t) {
      return new Position(t.Item1, t.Item2);
    }

    public void Deconstruct(out int x, out int y) {
      x = X;
      y = Y;
    }

    public override string ToString() {
      return $"({X},{Y})";
    }

    public static Position operator +(Position first, Position second) {
      return (first.X + second.X, first.Y + second.Y);
    }

    public static Position Invalid => new(-1, -1);
    
    public override bool Equals(object other) {
      return (other is Position position) && this.Equals(position);
    }
    
    public bool Equals(Position other) {
      return this.X == other.X && this.Y == other.Y;
    }
    
    public override int GetHashCode() {
      return (this.X, this.Y).GetHashCode();
    }

  }
}
