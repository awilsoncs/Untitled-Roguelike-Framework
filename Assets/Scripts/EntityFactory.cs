using UnityEngine;

[CreateAssetMenu]
public class EntityFactory : ScriptableObject {
    private int idCounter = 0;
    [SerializeField] Entity[] prefabs;

    public Entity Get (int i) {
        var entity = Instantiate(prefabs[i]);
        entity.ID = idCounter++;
        entity.EntityType = i;
        return entity;
    }
}