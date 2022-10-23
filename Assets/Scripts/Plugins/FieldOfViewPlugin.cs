using UnityEngine;
public abstract class FieldOfViewPlugin : ScriptableObject, IFieldOfViewPlugin {
    public abstract IFieldOfView Impl {get;}
}