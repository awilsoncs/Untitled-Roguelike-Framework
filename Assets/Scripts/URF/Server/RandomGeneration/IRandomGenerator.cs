// Provide a port for random number generation.

using System.Runtime.Serialization;
using URF.Common.Persistence;

namespace URF.Server.RandomGeneration {
  public interface IRandomGenerator : IPersistableObject {

    int GetInt(int begin, int end);

    void Rotate();

  }
}
