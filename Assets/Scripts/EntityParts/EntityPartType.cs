public enum EntityPartType {
    PlayerBrain,
}

public static class EntityPartTypeMethods {
    public static IEntityPart GetInstance (this EntityPartType type) {
        switch (type) {
            case EntityPartType.PlayerBrain:
                return EntityPartPool<PlayerBrain>.Get();
        }

        UnityEngine.Debug.Log("Forgot to support " + type);
        return null;
    }
}