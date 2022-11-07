namespace URFCommon {
    public struct Position {
        public int X {get;}
        public int Y {get;}

        public Position(int x, int y) {
            X = x;
            Y = y;
        }

        public static implicit operator Position((int, int) t) => new(t.Item1, t.Item2);
        public void Deconstruct(out int x, out int y) {
            x = X;
            y = Y; 
        }

        public override string ToString() {
            return $"({X},{Y})";
        }

        public static Position operator +(Position first, Position second) => 
            (first.X + second.X, first.Y + second.Y);
    }
}
