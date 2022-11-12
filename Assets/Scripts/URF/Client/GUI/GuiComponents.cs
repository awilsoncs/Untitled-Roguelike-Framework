using System;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;

namespace URF.Client.GUI {
  [Serializable]
  public class GuiComponents {

    [SerializeField] private HealthBar healthBar;

    [SerializeField] private MessageBox messageBox;

    public HealthBar HealthBar => healthBar;

    public MessageBox MessageBox => messageBox;

  }
}
