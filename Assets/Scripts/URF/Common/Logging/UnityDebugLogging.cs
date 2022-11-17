using UnityEngine;

namespace URF.Common.Logging {
  public class UnityDebugLogging : ILogging {

    public void Log(string message) {
      Debug.Log($"SERVER LOG: {message}");
    }

  }
}
