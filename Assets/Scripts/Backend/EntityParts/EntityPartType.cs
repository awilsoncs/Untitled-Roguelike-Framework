public enum EntityPartType {
    Health 
}

public static class EntityPartTypeMethods {
    public static IEntityPart GetInstance (this EntityPartType type) {
        switch (type) {
            case EntityPartType.Health:
                return EntityPartPool<HealthPart>.Get();
            default:
                UnityEngine.Debug.Log("Forgot to support " + type);
                return null;
        }
    }
}