using System;
using UnityEngine;
using URF.Client.Pathfinding;
using URF.Common.Logging;
using URF.Game.Plugins;

namespace URF.Game {
  [Serializable]
  public class BackendPlugins {

    [SerializeField] private RandomGeneratorPlugin randomGeneratorPlugin;

    public RandomGeneratorPlugin RandomPlugin => randomGeneratorPlugin;

    [SerializeField] private FieldOfViewPlugin fieldOfViewPlugin;

    public FieldOfViewPlugin FieldOfViewPlugin => fieldOfViewPlugin;

    [SerializeField] private LoggingPlugin loggingPlugin;

    public LoggingPlugin LoggingPlugin => loggingPlugin;

    [SerializeField] private PathfindingPlugin pathfindingPlugin;

    public PathfindingPlugin PathfindingPlugin => pathfindingPlugin;

  }
}
