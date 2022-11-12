using UnityEngine;
using URF.Server.FieldOfView;

namespace URF.Game.Plugins
{
    [CreateAssetMenu]
    public class RaycastingFOVPlugin : FieldOfViewPlugin {
        public override IFieldOfView Impl {get;}
        public RaycastingFOVPlugin() {
            Impl = new RaycastingFOV();
        }
    }
}