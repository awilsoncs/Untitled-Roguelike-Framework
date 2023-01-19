// Provide a port for random number generation.

namespace URF.Algorithms {
  using URF.Common.Persistence;

  public interface IRandomGenerator : IPersistableObject {

    int GetInt(int begin, int end);

    void Rotate();

  }
}
