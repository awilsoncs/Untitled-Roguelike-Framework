public enum EntityPartType {
    Fighter 
}

public static class EntityPartTypeMethods {
    public static IEntityPart GetInstance (this EntityPartType type) {
        switch (type) {
            case EntityPartType.Fighter:
                return EntityPartPool<FighterPart>.Get();
            default:
                UnityEngine.Debug.Log("Forgot to support " + type);
                return null;
        }
    }
}