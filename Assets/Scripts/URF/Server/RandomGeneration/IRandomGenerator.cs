// Provide a port for random number generation.

namespace URF.Server.RandomGeneration {
  public interface IRandomGenerator {

    int GetInt(int begin, int end);

  }
}
