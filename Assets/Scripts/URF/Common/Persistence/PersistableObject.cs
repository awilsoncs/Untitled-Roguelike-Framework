using UnityEngine;

namespace URF.Common.Persistence {
  [DisallowMultipleComponent]
  public class PersistableObject : MonoBehaviour, IPersistableObject {

    public virtual void Save(GameDataWriter writer) {
      writer.Write(transform.localPosition);
      writer.Write(transform.localScale);
    }

    public virtual void Load(GameDataReader reader) {
      Transform thisTransform = transform;
      thisTransform.localPosition = reader.ReadVector3();
      thisTransform.localScale = reader.ReadVector3();
      thisTransform.localRotation = Quaternion.identity;
    }

  }
}
