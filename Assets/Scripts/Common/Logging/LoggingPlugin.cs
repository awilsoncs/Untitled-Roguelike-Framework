using UnityEngine;

public abstract class LoggingPlugin : ScriptableObject {
    public abstract ILogging Impl {get;}

}