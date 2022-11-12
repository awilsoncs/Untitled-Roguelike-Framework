using System;
using UnityEngine;
using URF.Common.Logging;
using URF.Game.Plugins;
using URF.Server.FieldOfView;

namespace URFFrontend {
    [Serializable]
    public class BackendPlugins
    {
        [SerializeField]
        [Tooltip("Random number generator")]
        public RandomGeneratorPlugin randomPlugin;
        [SerializeField]
        [Tooltip("Field of view calculation")]
        public FieldOfViewPlugin fieldOfViewPlugin;
        [SerializeField]
        [Tooltip("(Optional) plugin for logging")]
        public LoggingPlugin loggingPlugin;
        [SerializeField]
        [Tooltip("Pathfinding system")]
        public PathfindingPlugin pathfindingPlugin;
    }
}
