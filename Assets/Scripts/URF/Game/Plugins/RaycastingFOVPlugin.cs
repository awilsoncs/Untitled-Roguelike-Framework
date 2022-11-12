using UnityEngine;
using URF.Server.FieldOfView;

namespace URF.Game.Plugins {
  [CreateAssetMenu]
  public class RaycastingFovPlugin : FieldOfViewPlugin {

    public override IFieldOfView Impl { get; }

    public RaycastingFovPlugin() {
      Impl = new RaycastingFov();
    }

  }
}
