using UnityEngine;

[CreateAssetMenu]
public class EntityFactory : ScriptableObject {
// todo implement pooling behavior
    private int idCounter = 0;
    [SerializeField] Entity[] prefabs;

    public Entity Get (int i) {
        // todo seems to maintain idCounter after a hard reset
        var entity = Instantiate(prefabs[i]);
        entity.ID = idCounter++;
        entity.EntityType = i;
        return entity;
    }

    public void Reclaim (Entity entity) {
        Destroy(entity.gameObject);
    }
}