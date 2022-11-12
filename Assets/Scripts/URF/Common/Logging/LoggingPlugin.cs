using UnityEngine;

namespace URF.Common.Logging {
  public abstract class LoggingPlugin : ScriptableObject {

    public abstract ILogging Impl { get; }

  }
}
