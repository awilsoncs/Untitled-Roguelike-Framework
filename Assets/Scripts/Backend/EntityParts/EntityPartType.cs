public enum EntityPartType {
}

public static class EntityPartTypeMethods {
    public static IEntityPart GetInstance (this EntityPartType type) {

        UnityEngine.Debug.Log("Forgot to support " + type);
        return null;
    }
}