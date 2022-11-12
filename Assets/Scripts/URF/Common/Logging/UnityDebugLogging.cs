using UnityEngine;

namespace URF.Common.Logging {
  public class UnityDebugLogging : ILogging {

    public void Log(string message) {
      // todo technically not only for the server
      Debug.Log($"SERVER LOG: {message}");
    }

  }
}
