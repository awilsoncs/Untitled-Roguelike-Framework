using UnityEngine;

[DisallowMultipleComponent]
public class PersistableObject : MonoBehaviour, IPersistableObject {
    public virtual void Save (GameDataWriter writer) {
        writer.Write(transform.localPosition);
        writer.Write(transform.localScale);
    }

    public virtual void Load (GameDataReader reader) {
        transform.localPosition = reader.ReadVector3();
        transform.localScale = reader.ReadVector3();
        transform.localRotation = Quaternion.identity;
    }
}