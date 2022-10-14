using UnityEngine;

[CreateAssetMenu]
public class EntityFactory : ScriptableObject {
    [SerializeField] Entity[] prefabs;

    public Entity Get (int i) {
        return Instantiate(prefabs[i]); 
    }
}