using UnityEngine;

[CreateAssetMenu(menuName = "URF Plugins/Logging/UnityDebugLogging")]
public class UnityDebugLoggingPlugin : LoggingPlugin {
    ILogging _impl;
    public override ILogging Impl => _impl;

    public UnityDebugLoggingPlugin() {
        _impl = new UnityDebugLogging();
    }
}