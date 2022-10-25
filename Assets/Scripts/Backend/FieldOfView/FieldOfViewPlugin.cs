using UnityEngine;

public abstract class FieldOfViewPlugin : ScriptableObject {
    public abstract IFieldOfView Impl {get;}

}