public enum EntityPartType {
    Fighter,
    MonsterActor
}

public static class EntityPartTypeMethods {
    public static IEntityPart GetInstance (this EntityPartType type) {
        switch (type) {
            case EntityPartType.Fighter:
                return EntityPartPool<FighterPart>.Get();
            case EntityPartType.MonsterActor:
                return EntityPartPool<MonsterActor>.Get();
            default:
                UnityEngine.Debug.LogError("Forgot to support " + type);
                return null;
        }
    }
}