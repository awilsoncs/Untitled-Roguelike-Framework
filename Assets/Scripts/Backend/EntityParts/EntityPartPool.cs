using System.Collections.Generic;
using UnityEngine;

public static class EntityPartPool<T> where T : EntityPart, new() {
    static Stack<T> stack = new Stack<T>();

    public static T Get () {
        if (stack.Count > 0) {
            T part = stack.Pop();
// For all build specific behavior, see EntityPart::OnEnable
#if UNITY_EDITOR
            part.IsReclaimed = false;
#endif
            return part;
        }
#if UNITY_EDITOR
        return ScriptableObject.CreateInstance<T>();
#else
        return new T();
#endif
    }
 
    public static void Reclaim (T behavior) {
#if UNITY_EDITOR
        behavior.IsReclaimed = true;
#endif
        stack.Push(behavior);
    }
}