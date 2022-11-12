using UnityEngine;

namespace URF.Common.Logging {
  [CreateAssetMenu(menuName = "URF Plugins/Logging/UnityDebugLogging")]
  public class UnityDebugLoggingPlugin : LoggingPlugin {

    public override ILogging Impl { get; }

    public UnityDebugLoggingPlugin() {
      Impl = new UnityDebugLogging();
    }

  }
}
