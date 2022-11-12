using UnityEngine;
using URF.Server.FieldOfView;

namespace URF.Game.Plugins {
  public abstract class FieldOfViewPlugin : ScriptableObject {

    public abstract IFieldOfView Impl { get; }

  }
}
