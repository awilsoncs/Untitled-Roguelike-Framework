using UnityEngine;

public abstract class ClientUserInputPlugin : ScriptableObject {
    public abstract IClientUserInput Impl {get;}

}