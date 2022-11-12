using UnityEngine;
using URF.Server.FieldOfView;

namespace URF.Game.Plugins {
  [CreateAssetMenu(menuName = "URF Plugins/Field of View/Raycasting FOV")]
  public class RaycastingFovPlugin : FieldOfViewPlugin {

    public override IFieldOfView Impl { get; }

    public RaycastingFovPlugin() {
      Impl = new RaycastingFov();
    }

  }
}
