using System;
using UnityEngine;

namespace URF.Client.GUI {
  [Serializable]
  public class GuiComponents {

    [SerializeField] private HealthBar healthBar;

    [SerializeField] private MessageBox messageBox;

    public HealthBar HealthBar => healthBar;

    public MessageBox MessageBox => messageBox;

  }
}
