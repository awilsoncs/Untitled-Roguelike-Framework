using System;
using UnityEngine;

[Serializable]
public class BackendPlugins
{
    [SerializeField]
    [Tooltip("Random number generator")]
    public RandomGeneratorPlugin randomPlugin;
    [SerializeField]
    [Tooltip("Field of view calculation")]
    public FieldOfViewPlugin fieldOfViewPlugin;    
}
