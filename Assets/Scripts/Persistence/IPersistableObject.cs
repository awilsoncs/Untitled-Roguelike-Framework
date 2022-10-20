using UnityEngine;

public interface IPersistableObject {
    void Save (GameDataWriter writer);
    void Load (GameDataReader reader);
}