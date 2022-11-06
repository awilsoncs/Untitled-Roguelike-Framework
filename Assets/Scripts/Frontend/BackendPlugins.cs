using System;
using UnityEngine;

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
